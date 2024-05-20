namespace redditpoller.application
{
    /// <summary>
    /// Constants for redding polling application.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Minimum duration for a sample.
        /// </summary>
        public static readonly int MinSampleDuration = 30;

        /// <summary>
        /// Maximum duration for a sample.
        /// </summary>
        public static readonly int MaxSampleDuration = 600;

        /// <summary>
        /// Base URL for reddit that will be used to create fully qualified URLs to reddit posts.
        /// </summary>
        public static readonly string RedditBaseUrl = "https://www.reddit.com";
    }
}
