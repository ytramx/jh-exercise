using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Reddit;
using redditpoller.application.Sample.Commands;
using redditpoller.application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace redditpoller.unit
{
    public class TestBase
    {
        protected Mock<RedditClientMock> redditClientMock;
        protected Mock<IPersistenceService> persistMock;
        protected Mock<IRedditService> redditServiceMock;
        protected IMediator mediator;
        protected readonly string[] allowableSubredditNames;

        protected TestBase()
        {
            var services = new ServiceCollection();

            // Services
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateSample).Assembly));

            // Mocks
            this.persistMock = new Mock<IPersistenceService>();
            this.redditServiceMock = new Mock<IRedditService>();
            this.redditClientMock = new Mock<RedditClientMock>();

            services.AddSingleton<RedditClient>(this.redditClientMock.Object);
            services.AddSingleton<IPersistenceService>(this.persistMock.Object);
            services.AddSingleton<IRedditService>(this.redditServiceMock.Object);

            var provider = services.BuildServiceProvider();

            this.mediator = provider.GetRequiredService<IMediator>();
        }
    }
}
