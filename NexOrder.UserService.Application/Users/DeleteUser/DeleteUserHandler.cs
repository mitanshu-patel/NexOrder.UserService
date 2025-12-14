using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NexOrder.UserService.Application.Common;
using NexOrder.UserService.Domain;
using NexOrder.UserService.Infrastructure;
using NexOrder.UserService.Shared.Common;

namespace NexOrder.UserService.Application.Users.DeleteUser
{
    public class DeleteUserHandler : RequestHandlerBase<DeleteUserCommand, CustomResponse<DeleteUserResult>>
    {
        private readonly UsersContext dbContext;
        private readonly ILogger<DeleteUserHandler> logger;

        public DeleteUserHandler(UsersContext dbContext, ILogger<DeleteUserHandler> logger)
        {
            this.dbContext = dbContext;
            this.logger = logger;
        }
        protected async override Task<CustomResponse<DeleteUserResult>> ExecuteCommandAsync(DeleteUserCommand command)
        {
            try
            {
                this.logger.LogInformation("DeleteUserHandler: ExecuteCommandAsync execution started");
                var userDetail = await dbContext.Users.Where(v => v.Id == command.UserId).FirstOrDefaultAsync();

                if (userDetail == null)
                {
                    this.logger.LogError("User details for Id:{userId} not found", command.UserId);
                    return CustomHttpResult.NotFound<DeleteUserResult>("User not found");
                }

                userDetail.IsDeleted = true;
              
                await this.dbContext.SaveChangesAsync();

                this.logger.LogInformation("DeleteUserHandler: ExecuteCommandAsync execution completed and deleted user");

                return CustomHttpResult.Ok(new DeleteUserResult());
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "DeleteUserHandler: exception occurred with message:{message}", ex.Message);
                throw;
            }
        }

        protected override CustomResponse<DeleteUserResult> GetValidationFailedResult(ValidationResult validationResult)
        {
            return validationResult.GetValidationResult<DeleteUserResult>();
        }

        protected override IValidator<DeleteUserCommand> GetValidator()
        {
            var validator = new InlineValidator<DeleteUserCommand>();
            validator.RuleFor(v => v.UserId);
            return validator;
        }
    }
}
