namespace redditpoller.application.Services.Models
{
    /// <summary>
    /// A class that represents the analysis being done for the subreddit sample.
    /// </summary>
    public class SampleAnalysis
    {
        /// <summary>
        /// Max upvote value found.
        /// </summary>
        public int MaxUpvotes { get; set; }

        /// <summary>
        /// Posts that match the MaxUpvotes value.
        /// </summary>
        public IEnumerable<BasePostData> MostUpvotedPosts { get; set; }

        /// <summary>
        /// Max number of posts by distinct authors in sample.
        /// </summary>
        public int MaxPostCount { get; set; }

        /// <summary>
        /// Authors that posted MaxPostCount posts during sample.
        /// </summary>
        public IEnumerable<string> ProlificAuthors { get; set; }
    }
}
