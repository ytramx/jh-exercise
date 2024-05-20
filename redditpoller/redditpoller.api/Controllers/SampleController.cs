using MediatR;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using redditpoller.application.Sample.Commands;
using redditpoller.application.Sample.Models;
using redditpoller.application.Sample.Queries;
using System.Net;

namespace redditpoller.api.Controllers
{
    /// <summary>
    /// A collection of endpoints to create, delete, and read samples of subreddit data polling.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SampleController : ControllerBase
    {
        private IMediator mediator;

        /// <summary>
        /// Initializes a new instance of the <see cref="SampleController"/> class.
        /// </summary>
        /// <param name="mediator">The mediator</param>
        public SampleController(IMediator mediator)
        {
            _ = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.mediator = mediator;
        }

        /// <summary>
        /// Create a new sample for the specified subreddit for the specified duration.
        /// </summary>
        /// <param name="sampleId">Identifier for sample.  Will be auto-generated if not provided.</param>
        /// <param name="command">Command representing the sample creation.</param>
        /// <returns>A result with SampleId and a Created HTTP status.  Bad Request otherwise.</returns>
        [HttpPost("{sampleId?}")]
        [ProducesResponseType(typeof(CreateSampleResult), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(Exception), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateSample([FromBody]CreateSample.Command command, [FromRoute]string sampleId = null)
        {
            _ = command ?? throw new ArgumentNullException(nameof(command));

            command.SampleId = sampleId ?? Guid.NewGuid().ToString();
            var result = await this.mediator.Send(command);

            if(result != null && result.Success)
            {
                return this.Created(new Uri($"{this.Request.GetDisplayUrl()}{result.SampleId}"), result);
            }
            return this.BadRequest(result);
        }

        [HttpGet("{sampleId}")]
        public async Task<IActionResult> GetSample([FromRoute]string sampleId)
        {
            var sample = await this.mediator.Send(new GetSampleById.Query { SampleId = sampleId });

            if(sample != null)
            {
                return this.Ok(sample);
            }

            return this.NotFound();
        }
    }
}
