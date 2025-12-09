using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NexOrder.UserService.Application.Common;
using NexOrder.UserService.Application.Users.AddUser;
using NexOrder.UserService.Shared.Common;
using System.Net;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace NexOrder.UserService;

public class UserServiceFunction
{
    private readonly ILogger<UserServiceFunction> _logger;
    private readonly IMediator mediator;

    public UserServiceFunction(ILogger<UserServiceFunction> logger, IMediator mediator)
    {
        _logger = logger;
        this.mediator = mediator;
    }

    [Function("AddUser")]
    [OpenApiOperation(operationId: "Run", tags: new[] { "example" })]
    [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(AddUserCommand))]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(AddUserResult))]
    public async Task<IActionResult> AddUser([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/users/add")] HttpRequest req)
    {
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var data = JsonConvert.DeserializeObject<AddUserCommand>(requestBody);
        var result = await this.mediator.SendAsync<AddUserCommand, CustomResponse<AddUserResult>>(data);
        return result.GetResponse();
    }
}