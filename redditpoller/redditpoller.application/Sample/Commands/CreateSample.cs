using MediatR;
using redditpoller.application.Exceptions;
using redditpoller.application.Sample.Models;
using redditpoller.application.Services;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace redditpoller.application.Sample.Commands
{
    /// <summary>
    /// CreateSample CQRS container.
    /// </summary>
    public static class CreateSample
    {
        /// <summary>
        /// Command representing a poll for data from a specific subreddit for the provided duration.
        /// </summary>
        public class Command : IRequest<CreateSampleResult>
        {
            /// <summary>
            /// The name of the subreddit to poll.
            /// </summary>
            public string SubredditName { get; set; }

            /// <summary>
            /// The duration of the poll in seconds.  The the minimum is 30 seconds, and the maximum is 600 seconds.
            /// </summary>
            public int Duration { get; set; }

            /// <summary>
            /// The identifier for the sample.
            /// </summary>
            [JsonIgnore]
            public string SampleId { get; set; }
        }

        /// <summary>
        /// Handler class for CreateSample.
        /// </summary>
        public class Handler : IRequestHandler<Command, CreateSampleResult>
        {
            private readonly IRedditService redditService;

            /// <summary>
            /// Initializes a new instance of the <see cref="Handler"/> class.
            /// </summary>
            /// <param name="redditService">The reddit service.</param>
            public Handler(IRedditService redditService)
            {
                _ = redditService ?? throw new ArgumentNullException(nameof(redditService));

                this.redditService = redditService;
            }

            /// <summary>
            /// Handle function for command.
            /// </summary>
            /// <param name="command">Command to be handled.</param>
            /// <param name="cancellationToken">Cancel token for async operations.</param>
            /// <returns>CreateSampleResult with new sample ID and overall success.</returns>
            public async Task<CreateSampleResult> Handle(Command command, CancellationToken cancellationToken)
            {
                var retval = new CreateSampleResult
                {
                    SampleId = command.SampleId
                };

                if(this.ValidateCommand(command, retval) == false)
                {
                    return retval;
                }

                try
                {
                    this.redditService.AddRequest(new Services.Models.SampleRequest
                    {
                        SubredditName = command.SubredditName,
                        Duration = command.Duration,
                        SampleId = command.SampleId
                    });
                    retval.Success = true;
                    return retval;
                }
                catch (SampleIdConflictException)
                {
                    retval.Conflict = true;
                }                

                return await Task.FromResult(retval);
            }

            /// <summary>
            /// Validates command matches allowable inputs.
            /// </summary>
            /// <param name="command">Command being validated</param>
            /// <param name="result">Overall result for create command.</param>
            /// <returns>True if valid, false otherwise</returns>
            private bool ValidateCommand(Command command, CreateSampleResult result) 
            {
                if (Regex.IsMatch(command.SampleId, @"^[a-zA-Z0-9-]+$") == false)
                {
                    result.InvalidSampleId = true;
                    return false;
                }

                if(command.Duration < Constants.MinSampleDuration || command.Duration > Constants.MaxSampleDuration)
                {
                    result.InvalidDuration = true;
                    return false;
                }

                if (this.redditService.SubredditExists(command.SubredditName) == false)
                {
                    result.InvalidSubreddit = true;
                    return false;
                }

                return true;
            }
        }
    }
}
