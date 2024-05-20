using Microsoft.Extensions.Options;
using redditpoller.application.Configuration;
using redditpoller.application.Exceptions;
using redditpoller.application.Services.Models;
using System.Text.Json;

namespace redditpoller.application.Services
{
    /// <summary>
    /// Service the performs I/O of completed samples.
    /// </summary>
    public class PersistenceService : IPersistenceService
    {
        private readonly PersistenceConfiguration persistConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistenceService"/> class.
        /// </summary>
        /// <param name="persistConfigOptions">Config options for persistence.</param>
        public PersistenceService(IOptionsMonitor<PersistenceConfiguration> persistConfigOptions)
        {
            this.persistConfig = persistConfigOptions.CurrentValue;
            if(Directory.Exists(this.persistConfig.SaveLocation) == false)
            {
                Directory.CreateDirectory(this.persistConfig.SaveLocation);
            }
        }

        /// <summary>
        /// Retrieves a sample if it exists.
        /// </summary>
        /// <param name="sampleId">Id of sample.</param>
        /// <returns>Sample request if it exists, null otherwise.</returns>
        public SampleRequest RetrieveSample(string sampleId)
        {
            var saveLocation = Path.Combine(persistConfig.SaveLocation, $"{sampleId}.json");
            if(File.Exists(saveLocation))
            {
                var content = File.ReadAllText(saveLocation);
                return JsonSerializer.Deserialize<SampleRequest>(content);
            }
            return null;
        }

        /// <summary>
        /// Verifies if a sample id is unique.
        /// </summary>
        /// <param name="sampleId">Sample id to test.</param>
        /// <returns>True if id is unique, false otherwise.</returns>
        public bool CheckSampleIdUniqueness(string sampleId)
        {
            var saveLocation = Path.Combine(persistConfig.SaveLocation, $"{sampleId}.json");
            if (File.Exists(saveLocation))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Persists a sample to disk.
        /// </summary>
        /// <param name="request">Sample to persist.</param>
        public void SaveSample(SampleRequest request)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));

            var saveLocation = Path.Combine(persistConfig.SaveLocation, $"{request.SampleId}.json");
            if (File.Exists(saveLocation))
            {
                throw new SampleIdConflictException(request.SampleId);
            }
            var content = JsonSerializer.Serialize(request);
            File.AppendAllText(saveLocation, content);
        }
    }
}
