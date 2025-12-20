using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NexOrder.UserService.Application.Common;
using NexOrder.UserService.Application.Users.AddUser;
using NexOrder.UserService.Application.Users.AuthenticateUser;
using NexOrder.UserService.Application.Users.DeleteUser;
using NexOrder.UserService.Application.Users.GetUser;
using NexOrder.UserService.Application.Users.SearchUsers;
using NexOrder.UserService.Application.Users.UpdateUser;
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
    [OpenApiOperation(operationId: "AddUser", tags: new[] { "AddUser" }, Description = "Add new user.")]
    [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(AddUserCommand))]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(AddUserResult))]
    public async Task<IActionResult> AddUser([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/users")] HttpRequest req)
    {
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var data = JsonConvert.DeserializeObject<AddUserCommand>(requestBody);
        var result = await this.mediator.SendAsync<AddUserCommand, CustomResponse<AddUserResult>>(data);
        return result.GetResponse();
    }

    [Function("UpdateUser")]
    [OpenApiOperation(operationId: "UpdateUser", tags: new[] { "UpdateUser" }, Description = "Update user details for given user id.")]
    [OpenApiParameter(name:"userId", Type = typeof(int), Required = true)]
    [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(UpdateUserCriteria))]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(UpdateUserResult))]
    public async Task<IActionResult> UpdateUser([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "v1/users/{userId:int}")] HttpRequest req, int userId)
    {
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var data = JsonConvert.DeserializeObject<UpdateUserCriteria>(requestBody);
        var command = new UpdateUserCommand(userId, data);
        var result = await this.mediator.SendAsync<UpdateUserCommand, CustomResponse<UpdateUserResult>>(command);
        return result.GetResponse();
    }

    [Function("GetUser")]
    [OpenApiOperation(operationId: "GetUser", tags: new[] { "GetUser" }, Description = "Get user details for given user id.")]
    [OpenApiParameter(name: "userId", Type = typeof(int), Required = true)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(GetUserResult))]
    public async Task<IActionResult> GetUser([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/users/{userId:int}")] HttpRequest req, int userId)
    {
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var command = new GetUserQuery(userId);
        var result = await this.mediator.SendAsync<GetUserQuery, CustomResponse<GetUserResult>>(command);
        return result.GetResponse();
    }


    [Function("DeleteUser")]
    [OpenApiOperation(operationId: "DeleteUser", tags: new[] { "DeleteUser" }, Description = "Delete user details of given user id.")]
    [OpenApiParameter(name: "userId", Type = typeof(int), Required = true)]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(DeleteUserResult))]
    public async Task<IActionResult> DeleteUser([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "v1/users/{userId:int}")] HttpRequest req, int userId)
    {
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var command = new DeleteUserCommand(userId);
        var result = await this.mediator.SendAsync<DeleteUserCommand, CustomResponse<DeleteUserResult>>(command);
        return result.GetResponse();
    }

    [Function("SearchUsers")]
    [OpenApiOperation(operationId: "SearchUsers", tags: new[] { "SearchUsers" }, Description = "Search users for given criteria with pagination.")]
    [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(SearchUsersQuery))]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(SearchUsersResult))]
    public async Task<IActionResult> SearchUsers([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/users/search")] HttpRequest req)
    {
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var data = JsonConvert.DeserializeObject<SearchUsersQuery>(requestBody);
        var result = await this.mediator.SendAsync<SearchUsersQuery, CustomResponse<SearchUsersResult>>(data);
        return result.GetResponse();
    }

    [Function("AuthenticateUser")]
    [OpenApiOperation(operationId: "AuthenticateUser", tags: new[] { "AuthenticateUser" }, Description = "Authenticate user credentials.")]
    [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(AuthenticateUserCommand))]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(AuthenticateUserResult))]
    public async Task<IActionResult> AuthenticateUser([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/users/authenticate")] HttpRequest req)
    {
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var data = JsonConvert.DeserializeObject<AuthenticateUserCommand>(requestBody);
        var result = await this.mediator.SendAsync<AuthenticateUserCommand, CustomResponse<AuthenticateUserResult>>(data);
        return result.GetResponse();
    }
}