namespace redditpoller.application.Exceptions
{
    /// <summary>
    /// Exception that represents a new sample trying to be created with an existing sample id.
    /// </summary>
    public class SampleIdConflictException : Exception
    {
        /// <summary>
        /// Sample id that was duplicated.
        /// </summary>
        public string SampleId { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SampleIdConflictException"/> class.
        /// </summary>
        /// <param name="sampleId">The sample id.</param>

        public SampleIdConflictException(string sampleId)
        {
            SampleId = sampleId;
        }

        /// <summary>
        /// Exception message.
        /// </summary>
        public override string Message => $"Sample with id '{this.SampleId}' already exists.";
    }
}
