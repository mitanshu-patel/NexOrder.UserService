using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NexOrder.UserService.Application.Common;
using NexOrder.UserService.Application.Users.AddUser;
using NexOrder.UserService.Application.Users.GetUser.DTOs;
using NexOrder.UserService.Domain;
using NexOrder.UserService.Shared.Common;

namespace NexOrder.UserService.Application.Users.GetUser
{
    public class GetUserHandler : RequestHandlerBase<GetUserQuery, CustomResponse<GetUserResult>>
    {
        private readonly IUserRepo userRepo;
        private readonly ILogger<AddUserHandler> logger;
        public GetUserHandler(IUserRepo userRepo, ILogger<AddUserHandler> logger)
        {
            this.userRepo = userRepo;
            this.logger = logger;
        }

        protected async override Task<CustomResponse<GetUserResult>> ExecuteCommandAsync(GetUserQuery command)
        {
            try
            {
                this.logger.LogInformation("GetUserHandler: ExecuteCommandAsync execution started");
                var userDetail = await this.userRepo.GetUsers().Where(v => v.Id == command.UserId).Select(v => new GetUserDto
                {
                    Id = v.Id,
                    Name = v.Name,
                    Email = v.Email,
                }).FirstOrDefaultAsync();

                if(userDetail == null)
                {
                    this.logger.LogError("User details for Id:{userId} not found", command.UserId);
                    return CustomHttpResult.NotFound<GetUserResult>("User not found");
                }

                this.logger.LogInformation("GetUserHandler: ExecuteCommandAsync execution completed and fetched details");

                return CustomHttpResult.Ok(new GetUserResult(userDetail));
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "GetUserHandler: exception occurred with message:{message}", ex.Message);
                throw;
            }
        }

        protected override CustomResponse<GetUserResult> GetValidationFailedResult(ValidationResult validationResult)
        {
            return validationResult.GetValidationResult<GetUserResult>();
        }

        protected override IValidator<GetUserQuery> GetValidator()
        {
            var validator = new InlineValidator<GetUserQuery>();
            validator.RuleFor(v => v.UserId).GreaterThan(0);
            return validator;
        }
    }
}
