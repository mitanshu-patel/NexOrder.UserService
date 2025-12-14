using Microsoft.ApplicationInsights.Extensibility.Implementation;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Azure.WebJobs.Description;
using Microsoft.Azure.WebJobs.Host.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NexOrder.UserService.Application.Common;
using NexOrder.UserService.Application.Registrations;
using NexOrder.UserService.Domain;
using NexOrder.UserService.Infrastructure;

var builder = FunctionsApplication.CreateBuilder(args);
var configuration = new ConfigurationBuilder()
                    .AddEnvironmentVariables().Build();

builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();
builder.Services.RegisterHandlers();
builder.Services.AddScoped<IMediator, Mediator>();
builder.Services.AddDbContext<UsersContext>(
    v => v.UseSqlServer(configuration.GetConnectionString("SystemDbConnectionString"),
    b => b.MigrationsAssembly("NexOrder.UserService.Infrastructure")));
builder.Build().Run();
