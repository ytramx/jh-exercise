namespace redditpoller.application.Services.Models
{
    /// <summary>
    /// Class that represents posts being analyzed.
    /// </summary>
    public class BasePostData
    {
        /// <summary>
        /// Full post name.  A unique identifier.
        /// </summary>
        public string Fullname { get; set; }

        /// <summary>
        /// Total number of upvotes for the post.
        /// </summary>
        public int UpVotes { get; set; }

        /// <summary>
        /// Author of post.
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// Title of post.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// URL to post.
        /// </summary>
        public string Url { get; set; }
    }
}
