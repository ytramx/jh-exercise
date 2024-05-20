using redditpoller.application.Services.Models;

namespace redditpoller.application.Services
{
    public interface IRedditService
    {
        bool SubredditExists(string sub);
        void AddRequest(SampleRequest request);
        SampleRequest GetRequest(string sampleId);
    }
}
