using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NexOrder.UserService.Application.Common;
using NexOrder.UserService.Domain.Entities;
using NexOrder.UserService.Shared.Common;

namespace NexOrder.UserService.Application.Users.AddUser
{
    public class AddUserHandler : RequestHandlerBase<AddUserCommand, CustomResponse<AddUserResult>>
    {
        private readonly IUserRepo userRepo;
        private readonly ILogger<AddUserHandler> logger;
        public AddUserHandler(IUserRepo userRepo, ILogger<AddUserHandler> logger)
        {
            this.userRepo = userRepo;
            this.logger = logger;
        }
        protected override async Task<CustomResponse<AddUserResult>> ExecuteCommandAsync(AddUserCommand command)
        {
            try
            {
                this.logger.LogInformation("AddUserHandler: ExecuteCommandAsync execution started");
                var userExists = await this.userRepo.GetUsers()
                    .AnyAsync(u => u.Email == command.Email);

                if (userExists)
                {
                    this.logger.LogError("User with the email :{email} already exists.", command.Email);
                    return CustomHttpResult.BadRequest<AddUserResult>("User with the same email already exists.");
                }

                var user = new User
                {
                    Name = command.Name,
                    Email = command.Email,
                    Password = command.Password.ComputeSHA256Hash(),
                    CreatedAtUtc = DateTime.UtcNow,
                };

                await this.userRepo.AddUserAsync(user);

                this.logger.LogInformation("AddUserHandler: ExecuteCommandAsync execution successfully with Id:{userId}", user.Id);

                return CustomHttpResult.Ok(new AddUserResult(user.Id));
            }
           catch(Exception ex)
            {
                this.logger.LogError(ex, "AddUserHandler: exception occurred with message:{message}", ex.Message);
                throw;
            }
        }

        protected override CustomResponse<AddUserResult> GetValidationFailedResult(ValidationResult validationResult)
        {
            return validationResult.GetValidationResult<AddUserResult>();
        }

        protected override IValidator<AddUserCommand> GetValidator()
        {
            var validator = new InlineValidator<AddUserCommand>();
            validator.RuleFor(v => v.Email).NotEmpty();
            validator.RuleFor(v => v.Name).NotEmpty().MaximumLength(100);
            validator.RuleFor(v => v.Password).NotEmpty();
            return validator;
        }
    }
}
