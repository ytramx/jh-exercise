using Reddit.Controllers;
using System.Text.Json.Serialization;

namespace redditpoller.application.Services.Models
{
    /// <summary>
    /// Class that represents a requested sample of a subreddit.
    /// </summary>
    public class SampleRequest
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
        public string SampleId { get; set; }

        /// <summary>
        /// The time when the sample started.
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// The time when the sample ended.
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// Identifies if sample has been completed.
        /// </summary>
        public bool Completed { get; set; }

        /// <summary>
        /// The amount of successful samples for the subreddit.
        /// </summary>
        public int TotalSamplesCompleted { get; set; }

        /// <summary>
        /// Accumulated data for this sample.
        /// </summary>
        [JsonIgnore]
        public IEnumerable<PostData> PostData { get; private set; }

        /// <summary>
        /// Analysis of post data that includes upvotes and author prolificity.
        /// </summary>
        [JsonInclude]
        public SampleAnalysis SampleAnalysis { get; private set; }

        /// <summary>
        /// This will add a range of posts to PostData and remove duplicates by keeping newest version.
        /// </summary>
        /// <param name="posts">Range of posts to add.</param>
        public void MergePostData(IEnumerable<Post> posts)
        {
            if(PostData == null)
            {
                PostData = posts.Select(Projections.PostToPostData());
                return;
            }

            PostData = PostData.Concat(posts.Select(Projections.PostToPostData()))
                .GroupBy(p => p.Fullname)
                .Select(grp => grp.OrderByDescending(p => p.LastUpdated).First());

            this.UpdateAnalysis();
        }

        /// <summary>
        /// This fully replaces PostData with provided posts.
        /// </summary>
        /// <param name="posts">Range of posts.</param>
        public void ReplacePostData(IEnumerable<Post> posts)
        {
            PostData = posts.Select(Projections.PostToPostData());
            this.UpdateAnalysis();
        }

        /// <summary>
        /// This updates the analysis data for the sample.
        /// </summary>
        private void UpdateAnalysis()
        {
            if(this.SampleAnalysis == null)
            {
                this.SampleAnalysis = new SampleAnalysis();
            }

            this.SampleAnalysis.MaxUpvotes = this.PostData.Max(p => p.UpVotes);
            this.SampleAnalysis.MostUpvotedPosts = this.PostData.Where(p => p.UpVotes == this.SampleAnalysis.MaxUpvotes);
            var prolificAuthors = this.PostData
                .GroupBy(p => p.Author)
                .Select(grp => new { Author = grp.Key, PostCount = grp.Count() })
                .GroupBy(a => a.PostCount)
                .OrderByDescending(g => g.Key)
                .FirstOrDefault()?.Select(a => new { a.Author, a.PostCount })
                .ToList();

            this.SampleAnalysis.MaxPostCount = prolificAuthors.First().PostCount;
            this.SampleAnalysis.ProlificAuthors = prolificAuthors.Select(p => p.Author).ToList();
        }
    }
}
