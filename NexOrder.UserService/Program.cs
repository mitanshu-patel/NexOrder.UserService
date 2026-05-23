using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NexOrder.Framework.Core;
using NexOrder.Framework.Core.Common;
using NexOrder.UserService.Application;
using NexOrder.UserService.Application.Common;
using NexOrder.UserService.Application.Services;
using NexOrder.UserService.Infrastructure;
using NexOrder.UserService.Infrastructure.Helpers;
using NexOrder.UserService.Infrastructure.HttpClients;
using NexOrder.UserService.Infrastructure.Repos;
using Polly;
using Polly.Extensions.Http;
using Polly.RateLimiting;
using System.Reflection;
using System.Threading.RateLimiting;

var builder = FunctionsApplication.CreateBuilder(args);
var configuration = new ConfigurationBuilder()
                    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .Build();
var environment = configuration.GetValue<string>("ENVIRONMENT");
var isDevelopment = !string.IsNullOrEmpty(environment) && environment.Equals(
            "DEVELOPMENT",
            System.StringComparison.InvariantCultureIgnoreCase);

builder.ConfigureFunctionsWebApplication();
var appInsightsConnection = Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING");

builder.Services.AddNexOrderCustomLogging(isDevelopment, "NexOrder.UserService", appInsightsConnection);
builder.Services.AddMessageDeliveryService(options =>
{
    options.ServiceBusConnectionString = configuration["ServiceBusConnectionString"]
        ?? configuration.GetConnectionString("ServiceBusConnectionString")
        ?? string.Empty;
#if DEBUG
    options.WebProxyAddress = Environment.GetEnvironmentVariable("WebProxy") ?? string.Empty;
#endif
});

builder.Services.RegisterHandlers(Assembly.Load("NexOrder.UserService.Application"));

var connectionString = ConnectionStringsHelper.GetDbConnectionString();
builder.Services.AddDbContext<UsersContext>(
    v => v.UseSqlServer(connectionString,
    b => b.MigrationsAssembly("NexOrder.UserService.Infrastructure")));
builder.Services.AddScoped<IUserRepo, UserRepo>();
builder.Services.AddHttpClient<IAuthServiceClient, AuthServiceClient>(client =>
{
    client.BaseAddress =
        new Uri(Environment.GetEnvironmentVariable("APIM_BASE_URL"));
});

builder.Services
    .AddHealthChecks()
    .AddDbContextCheck<UsersContext>("UsersDb")
    .AddUrlGroup(new Uri($"{Environment.GetEnvironmentVariable("APIM_BASE_URL")}swagger.json"), name: "AuthService");

builder.Services.AddResiliencePipeline("authservice-pipeline", pipelineBuilder =>
{
    pipelineBuilder.AddTimeout(TimeSpan.FromSeconds(60))
    .AddRetry(new Polly.Retry.RetryStrategyOptions
    {
        ShouldHandle = new PredicateBuilder().Handle<HttpRequestException>(),
        Delay = TimeSpan.FromSeconds(2),
        UseJitter = true,
        MaxRetryAttempts = 3,
    })
    .AddCircuitBreaker(new Polly.CircuitBreaker.CircuitBreakerStrategyOptions
    {
        ShouldHandle = new PredicateBuilder().Handle<HttpRequestException>(),
        FailureRatio = 0.5, // Break if 50% of requests fail
        SamplingDuration = TimeSpan.FromSeconds(10),
        MinimumThroughput = 8,
        BreakDuration = TimeSpan.FromSeconds(15) // Stay open for 15s
    });
    //.AddRateLimiter(new TokenBucketRateLimiter(
    //    new TokenBucketRateLimiterOptions
    //    {
    //        TokenLimit = 100,
    //        TokensPerPeriod = 50,
    //        ReplenishmentPeriod = TimeSpan.FromMinutes(1),
    //        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
    //        QueueLimit = 10
    //    }));
    });

var app = builder.Build();
if (builder.Configuration.GetValue<bool>("RunMigration"))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<UsersContext>();
    db.Database.Migrate();
    //return; // Exit after migration
}

app.Run();
