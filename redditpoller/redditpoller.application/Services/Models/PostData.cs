namespace redditpoller.application.Services.Models
{
    /// <summary>
    /// Class that represents posts being analyzed.
    /// </summary>
    public class PostData : BasePostData
    {
        /// <summary>
        /// Last time this post was polled.
        /// </summary>
        public DateTime LastUpdated { get; set; }
    }
}
