using Moq;
using redditpoller.application.Sample.Queries;
using redditpoller.application.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace redditpoller.unit.Sample
{
    public class GetSampleTests : TestBase
    {
        [Fact]
        public async Task GetSample_Returns_Null_When_Service_Returns_Null()
        {
            this.redditServiceMock.Setup(rs => rs.GetRequest(It.IsAny<string>())).Returns<SampleRequest>(null);

            var result = await this.mediator.Send(new GetSampleById.Query {  SampleId = Guid.NewGuid().ToString() });
            Assert.Null(result);
        }

        [Fact]
        public async Task GetSample_Returns_Expected_SampleId()
        {
            var expectedSampleId = Guid.NewGuid().ToString();
            this.redditServiceMock.Setup(rs => rs.GetRequest(It.IsAny<string>())).Returns(new SampleRequest
            {
                SampleId = expectedSampleId
            });

            var result = await this.mediator.Send(new GetSampleById.Query { SampleId = expectedSampleId });
            Assert.Equal(expectedSampleId, result.SampleId);
        }
    }
}
