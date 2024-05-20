using Microsoft.Extensions.Options;
using Reddit;
using redditpoller.application.Configuration;
using redditpoller.application.Sample.Commands;
using redditpoller.application.Services;

namespace redditpoller.api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers(options =>
                options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateSample).Assembly));

            var redditConfig = builder.Configuration.GetSection("RedditConfiguration");
            var pollingConfig = builder.Configuration.GetSection("PollingConfiguration");
            var persistConfig = builder.Configuration.GetSection("PersistenceConfiguration");
            builder.Services.Configure<RedditConfiguration>(redditConfig);
            builder.Services.Configure<PollingConfiguration>(pollingConfig);
            builder.Services.Configure<PersistenceConfiguration>(persistConfig);
            builder.Services.AddSingleton<IRedditService, RedditService>();
            builder.Services.AddSingleton<RedditClient>(resolver =>
            {
                var redditConfig = resolver.GetRequiredService<IOptionsMonitor<RedditConfiguration>>().CurrentValue;
                return new RedditClient(appId: redditConfig.AccessToken, accessToken: redditConfig.AccessToken);
            });
            builder.Services.AddTransient<IPersistenceService, PersistenceService>();            

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}