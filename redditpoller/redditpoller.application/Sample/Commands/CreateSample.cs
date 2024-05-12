using FluentValidation;
using MediatR;
using redditpoller.application.Sample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace redditpoller.application.Sample.Commands
{
    /// <summary>
    /// CreateSample CQRS package.
    /// </summary>
    public static class CreateSample
    {
        /// <summary>
        /// Command representing a poll for data from a specific subreddit for the provided duration.
        /// </summary>
        public class Command : IRequest<CreateSampleResult>
        {
            /// <summary>
            /// The name of the subreddit to poll.
            /// </summary>
            public string SubredditName { get; set; }

            /// <summary>
            /// The duration of the poll in seconds.  The the minimu is 15 seconds, and the maximum is 120 seconds.
            /// </summary>
            public int Duration { get; set; }

            /// <summary>
            /// The identifier for the sample.
            /// </summary>
            [JsonIgnore]
            public string SampleId { get; set; }
        }

        /// <summary>
        /// Validator for CreateSample.Command.
        /// </summary>
        public class CommandValidator : AbstractValidator<Command>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="CommandValidator"/> class.
            /// </summary>
            public CommandValidator() 
            {
                this.RuleFor(c => c.SubredditName).NotEmpty();
                this.RuleFor(c => c.Duration).LessThanOrEqualTo(Constants.MaxSampleDuration);
            }
        }

        /// <summary>
        /// Handler class for CreateSample.
        /// </summary>
        public class Handler : IRequestHandler<Command, CreateSampleResult>
        {
            /// <summary>
            /// Handle function for command.
            /// </summary>
            /// <param name="command">Command to be handled.</param>
            /// <param name="cancellationToken">Cancel token for async operations.</param>
            /// <returns>CreateSampleResult with new sample ID and overall success.</returns>
            public async Task<CreateSampleResult> Handle(Command command, CancellationToken cancellationToken)
            {
                return await Task.FromResult(new CreateSampleResult
                {
                    SampleId = command.SampleId,
                    Success = true
                });
            }
        }
    }
}
