using redditpoller.application.Services.Models;

namespace redditpoller.application.Services
{
    /// <summary>
    /// Service the performs I/O of completed samples.
    /// </summary>
    public interface IPersistenceService
    {
        /// <summary>
        /// Retrieves a sample if it exists.
        /// </summary>
        /// <param name="sampleId">Id of sample.</param>
        /// <returns>Sample request if it exists, null otherwise.</returns>
        SampleRequest RetrieveSample(string sampleId);

        /// <summary>
        /// Verifies if a sample id is unique.
        /// </summary>
        /// <param name="sampleId">Sample id to test.</param>
        /// <returns>True if id is unique, false otherwise.</returns>
        bool CheckSampleIdUniqueness(string sampleId);

        /// <summary>
        /// Persists a sample.
        /// </summary>
        /// <param name="request">Sample to persist.</param>
        void SaveSample(SampleRequest request);
    }
}
