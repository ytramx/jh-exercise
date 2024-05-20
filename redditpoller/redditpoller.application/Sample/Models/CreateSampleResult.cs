namespace redditpoller.application.Sample.Models
{
    /// <summary>
    /// Class representing the result of a CreateSample action.
    /// </summary>
    public class CreateSampleResult
    {
        /// <summary>
        /// The identifier for the sample to be used later for retrieving results.
        /// </summary>
        public string SampleId { get; set; }

        /// <summary>
        /// Success indicator for creation of new sample request.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Sample Id is not unique.
        /// </summary>
        public bool Conflict { get; set; }

        /// <summary>
        /// Sample Id contains invalid characters.
        /// </summary>
        public bool InvalidSampleId { get; set; }

        /// <summary>
        /// Subreddit does not exist.
        /// </summary>
        public bool InvalidSubreddit { get; set; }

        /// <summary>
        /// Requested duration falls outside allowable range.
        /// </summary>
        public bool InvalidDuration { get; set; }
    }
}
