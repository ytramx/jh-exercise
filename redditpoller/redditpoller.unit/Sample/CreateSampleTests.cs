using MediatR;
using Moq;
using Reddit.Things;
using redditpoller.application;
using redditpoller.application.Sample.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace redditpoller.unit.Sample
{
    public class CreateSampleTests : TestBase
    {
        [Theory]
        [InlineData("asdf!@#$")]
        [InlineData("ASDF:qwerty")]
        [InlineData("thisseemsokay,maybe")]
        public async Task CreateSample_Id_Invalid_Theory(string sampleId)
        {
            var command = new CreateSample.Command
            {
                SampleId = sampleId,
                Duration = 60,
                SubredditName = string.Empty
            };

            var result = await this.mediator.Send(command);

            Assert.True(result.InvalidSampleId);
            Assert.False(result.Success);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(601)]
        [InlineData(-1)]
        public async Task CreateSample_InvalidDurations(int duration)
        {
            var command = new CreateSample.Command
            {
                SampleId = Guid.NewGuid().ToString(),
                Duration = duration,
                SubredditName = string.Empty
            };

            var result = await this.mediator.Send(command);

            Assert.True(result.InvalidDuration);
            Assert.False(result.Success);
        }

        // Stalled out here do to non-extensibility of RedditClient.  Would need to revisit.
    }
}
