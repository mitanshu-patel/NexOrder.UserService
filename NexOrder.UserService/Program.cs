using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NexOrder.UserService.Application;
using NexOrder.UserService.Application.Common;
using NexOrder.UserService.Application.Registrations;
using NexOrder.UserService.Application.Services;
using NexOrder.UserService.Infrastructure;
using NexOrder.UserService.Infrastructure.Helpers;
using NexOrder.UserService.Infrastructure.HttpClients;
using NexOrder.UserService.Infrastructure.Repos;
using NexOrder.UserService.Infrastructure.Services;

var builder = FunctionsApplication.CreateBuilder(args);
var configuration = new ConfigurationBuilder()
                    .AddEnvironmentVariables().Build();

builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();
builder.Services.RegisterHandlers();
builder.Services.AddScoped<IMediator, Mediator>();
builder.Services.AddSingleton<IMessageDeliveryService, MessageDeliveryService>();
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
