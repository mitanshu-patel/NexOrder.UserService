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
using System.Reflection;

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

var app = builder.Build();
if (builder.Configuration.GetValue<bool>("RunMigration"))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<UsersContext>();
    db.Database.Migrate();
    //return; // Exit after migration
}

app.Run();
