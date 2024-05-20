using Microsoft.Extensions.Options;
using Reddit;
using Reddit.Exceptions;
using redditpoller.application.Configuration;
using redditpoller.application.Exceptions;
using redditpoller.application.Services.Models;
using System.Collections.Concurrent;

namespace redditpoller.application.Services
{
    /// <summary>
    /// Service representing the interactions with the reddit API.
    /// </summary>
    public class RedditService : IRedditService
    {
        private readonly ConcurrentQueue<SampleRequest> pendingRequests;
        private readonly RedditClient redditClient;
        private readonly IPersistenceService persistenceService;
        private readonly PollingConfiguration pollingConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedditService"/> class.
        /// </summary>
        /// <param name="redditClient">Reddit client from Reddit.NET Nuget package.</param>
        /// <param name="persistenceService">Service for I/O of completed samples.</param>
        /// <param name="pollOptionsMonitor">Options monitor for polling configuration.</param>
        public RedditService(
            RedditClient redditClient, 
            IPersistenceService persistenceService, 
            IOptionsMonitor<PollingConfiguration> pollOptionsMonitor)
        {
            pendingRequests = new ConcurrentQueue<SampleRequest>();
            this.redditClient = redditClient;
            this.persistenceService = persistenceService;
            this.pollingConfig = pollOptionsMonitor.CurrentValue;
            this.StartBackgroundTask();
        }

        /// <summary>
        /// Adds a request to the pending requests for this service.
        /// </summary>
        /// <param name="request">Request to be added.</param>
        public void AddRequest(SampleRequest request)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));

            if (this.SubredditExists(request.SubredditName) == false)
            {
                throw new RedditNotFoundException($"{request.SubredditName} does not exist");
            }

            if(this.pendingRequests.Any(r => r.SampleId == request.SampleId) ||
                this.persistenceService.CheckSampleIdUniqueness(request.SampleId) == false)
            {
                throw new SampleIdConflictException(request.SampleId);
            }

            pendingRequests.Enqueue(request);
            return;
        }

        /// <summary>
        /// Get a request by its id.
        /// </summary>
        /// <param name="sampleId">Id for sample.</param>
        /// <returns>SampleRequest or null if it does not exist.</returns>
        public SampleRequest GetRequest(string sampleId)
        {
            var request = this.pendingRequests.FirstOrDefault(r => r.SampleId == sampleId);
            if(request != null)
            {
                return request;
            }

            request = this.persistenceService.RetrieveSample(sampleId);
            return request;
        }

        /// <summary>
        /// Tests for the existence of a subreddit.
        /// </summary>
        /// <param name="sub">Name of subreddit</param>
        /// <returns>True if exists, false otherwise</returns>
        public bool SubredditExists(string sub)
        {
            return this.SubredditExistsInternal(sub);
        }

        /// <summary>
        /// A recursive subreddit search that allows for multiple attempts if a specific exception occurs.
        /// </summary>
        /// <param name="sub">Name of subreddit</param>
        /// <param name="attempt">Attempt number</param>
        /// <returns>True if exists, false otherwise</returns>
        private bool SubredditExistsInternal(string sub, int attempt = 0)
        {
            try
            {
                var test = this.redditClient.SearchSubredditNames(sub, exact: true);
                return true;
            }
            catch (RedditNotFoundException)
            {
                return false;
            }
            catch(RedditNoResponseException)
            {
                attempt++;
                if(attempt <= Constants.MaxRetries)
                {
                    return SubredditExistsInternal(sub, attempt);
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Kicks off the background task that will perform the polling.
        /// </summary>
        private void StartBackgroundTask()
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            TimeSpan interval = TimeSpan.FromSeconds(this.pollingConfig.Interval);

            Task.Run(async () =>
            {
                while (!cancellationTokenSource.Token.IsCancellationRequested)
                {
                    ProcessRequests();
                    await Task.Delay(interval, cancellationTokenSource.Token);
                }
            }, cancellationTokenSource.Token);
        }

        /// <summary>
        /// Processes all requests in parallel.
        /// </summary>
        private void ProcessRequests()
        {
            if (this.pendingRequests.Count == 0)
            {
                return;
            }

            Parallel.ForEach(pendingRequests, (request) => ProcessSingleRequest(request));
        }

        /// <summary>
        /// Processes an individual sample request.
        /// </summary>
        /// <param name="request">The sample request being processed.</param>
        private void ProcessSingleRequest(SampleRequest request)
        {
            if (request.StartTime.HasValue == false)
            {
                request.StartTime = DateTime.Now;
            }
            // We'll use this at the end.  Even if this time has passed, we will process one last time.
            var predictedEndTime = request.StartTime.Value.AddSeconds(request.Duration);

            var sub = this.redditClient.Subreddit(request.SubredditName);
            var posts = sub.Posts.New.GetRange(0, this.pollingConfig.RangeSize);
            request.MergePostData(posts);

            request.TotalSamplesCompleted++;

            if (predictedEndTime < DateTime.Now)
            {
                this.CompleteSample(request);
            }
        }

        /// <summary>
        /// After a sample's duration has elapsed, this method will set the appropriate values and persist the sample.
        /// </summary>
        /// <param name="request">The sample request being completed.</param>
        private void CompleteSample(SampleRequest request)
        {
            request.EndTime = DateTime.Now;
            request.Completed = true;

            // This should get the latest info for all posts we have gathered for this sample.
            var finalizedPosts = this.redditClient.GetPosts(request.PostData.Select(p => p.Fullname).ToList());
            request.ReplacePostData(finalizedPosts);
            this.persistenceService.SaveSample(request);

            this.pendingRequests.TryDequeue(out request);
        }
    }
}
