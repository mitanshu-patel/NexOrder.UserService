using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NexOrder.UserService.Application.Common;
using NexOrder.UserService.Domain;
using NexOrder.UserService.Infrastructure;
using NexOrder.UserService.Shared.Common;

namespace NexOrder.UserService.Application.Users.UpdateUser
{
    public class UpdateUserHandler : RequestHandlerBase<UpdateUserCommand, CustomResponse<UpdateUserResult>>
    {
        private readonly UsersContext dbContext;
        private readonly ILogger<UpdateUserHandler> logger;

        public UpdateUserHandler(UsersContext dbContext, ILogger<UpdateUserHandler> logger)
        {
            this.dbContext = dbContext;
            this.logger = logger;
        }

        protected async override Task<CustomResponse<UpdateUserResult>> ExecuteCommandAsync(UpdateUserCommand command)
        {
            try
            {
                this.logger.LogInformation("UpdateUserHandler: ExecuteCommandAsync execution started");
                var userDetail = await dbContext.Users.Where(v => v.Id == command.UserId).FirstOrDefaultAsync();

                if (userDetail == null)
                {
                    this.logger.LogError("User details for Id:{userId} not found", command.UserId);
                    return CustomHttpResult.NotFound<UpdateUserResult>("User not found");
                }

                userDetail.Name = command.Criteria.Name;
                userDetail.Email = command.Criteria.Email;
                userDetail.Password = command.Criteria.Password.ComputeSHA256Hash();

                await this.dbContext.SaveChangesAsync();

                this.logger.LogInformation("UpdateUserHandler: ExecuteCommandAsync execution completed and saved details");

                return CustomHttpResult.Ok(new UpdateUserResult());
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "UpdateUserHandler: exception occurred with message:{message}", ex.Message);
                throw;
            }
        }

        protected override CustomResponse<UpdateUserResult> GetValidationFailedResult(ValidationResult validationResult)
        {
            return validationResult.GetValidationResult<UpdateUserResult>();
        }

        protected override IValidator<UpdateUserCommand> GetValidator()
        {
            var validator = new InlineValidator<UpdateUserCommand>();
            validator.RuleFor(v => v.UserId).GreaterThan(0);
            validator.RuleFor(v => v.Criteria).NotNull();
            validator.RuleFor(v => v.Criteria.Email).NotEmpty();
            validator.RuleFor(v => v.Criteria.Name).NotEmpty().MaximumLength(100);
            validator.RuleFor(v => v.Criteria.Password).NotEmpty();
            return validator;
        }
    }
}
