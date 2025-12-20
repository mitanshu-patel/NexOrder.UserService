using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NexOrder.UserService.Application.Common;
using NexOrder.UserService.Application.Services;
using NexOrder.UserService.Domain;
using NexOrder.UserService.Messages.Events;
using NexOrder.UserService.Shared.Common;

namespace NexOrder.UserService.Application.Users.UpdateUser
{
    public class UpdateUserHandler : RequestHandlerBase<UpdateUserCommand, CustomResponse<UpdateUserResult>>
    {
        private readonly IUserRepo userRepo;
        private readonly ILogger<UpdateUserHandler> logger;
        private readonly IMessageDeliveryService messageDeliveryService;
        public UpdateUserHandler(IUserRepo userRepo, ILogger<UpdateUserHandler> logger, IMessageDeliveryService messageDeliveryService)
        {
            this.userRepo = userRepo;
            this.logger = logger;
            this.messageDeliveryService = messageDeliveryService;
        }

        protected async override Task<CustomResponse<UpdateUserResult>> ExecuteCommandAsync(UpdateUserCommand command)
        {
            try
            {
                this.logger.LogInformation("UpdateUserHandler: ExecuteCommandAsync execution started");
                var userDetail = await userRepo.GetUsers().Where(v => v.Id == command.UserId).FirstOrDefaultAsync();

                if (userDetail == null)
                {
                    this.logger.LogError("User details for Id:{userId} not found", command.UserId);
                    return CustomHttpResult.NotFound<UpdateUserResult>("User not found");
                }

                userDetail.Name = command.Criteria.Name;
                userDetail.Email = command.Criteria.Email;
                userDetail.Password = command.Criteria.Password.ComputeSHA256Hash();

                await this.userRepo.UpdateUserAsync(userDetail);
                await this.messageDeliveryService.PublishMessageAsync(new UserUpdated(userDetail.Id, userDetail.Email, userDetail.Name), UserServiceTopic.TopicName);

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
