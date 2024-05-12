using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using redditpoller.api.Models;
using redditpoller.application.Sample.Commands;
using redditpoller.application.Sample.Models;
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
        [ProducesResponseType(typeof(ResponseContainer<CreateSampleResult>), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ResponseContainer<Exception>), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateSample([FromBody]CreateSample.Command command, [FromRoute]string sampleId = null)
        {
            _ = command ?? throw new ArgumentNullException(nameof(command));

            command.SampleId = sampleId ?? Guid.NewGuid().ToString();
            var result = await this.mediator.Send(command);

            if(result != null && result.Success)
            {
                return this.Created(new Uri(this.Request.GetDisplayUrl()), result);
            }
            return this.BadRequest(result);
        }
    }
}
