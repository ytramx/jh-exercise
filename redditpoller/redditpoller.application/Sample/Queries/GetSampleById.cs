using MediatR;
using redditpoller.application.Services;
using redditpoller.application.Services.Models;

namespace redditpoller.application.Sample.Queries
{
    /// <summary>
    /// CQRS container for retrieving a sample by its id.
    /// </summary>
    public static class GetSampleById
    {
        /// <summary>
        /// Query representing input parameters for retrieval.
        /// </summary>
        public class Query : IRequest<SampleRequest>
        {
            /// <summary>
            /// Id of sample to retrieve.
            /// </summary>
            public string SampleId { get; set; }
        }

        /// <summary>
        /// Handler class for sample retrieval.
        /// </summary>
        public class Handler : IRequestHandler<Query, SampleRequest>
        {
            private readonly IRedditService redditService;

            /// <summary>
            /// Initializes a new instance of the <see cref="Handler"/> class.
            /// </summary>
            /// <param name="redditService">The reddit service.</param>
            public Handler(IRedditService redditService)
            {
                this.redditService = redditService;
            }

            /// <summary>
            /// Handle function to retrieve sample.
            /// </summary>
            /// <param name="request">Sample id.</param>
            /// <param name="cancellationToken">Cancellation token.</param>
            /// <returns>Sample request if found, null otherwise.</returns>
            public async Task<SampleRequest> Handle(Query request, CancellationToken cancellationToken)
            {
                return await Task.FromResult(this.redditService.GetRequest(request.SampleId));
            }
        }
    }
}
