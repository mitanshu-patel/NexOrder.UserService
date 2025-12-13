using FluentValidation;
using FluentValidation.Results;
using FluentValidation.TestHelper;
using Microsoft.EntityFrameworkCore;
using NexOrder.UserService.Application.Common;
using NexOrder.UserService.Domain;
using NexOrder.UserService.Domain.Entities;
using NexOrder.UserService.Shared.Common;

namespace NexOrder.UserService.Application.Users.AddUser
{
    public class AddUserHandler : RequestHandlerBase<AddUserCommand, CustomResponse<AddUserResult>>
    {
        private readonly UsersContext dbContext;
        public AddUserHandler(UsersContext usersContext)
        {
            this.dbContext = usersContext;
        }
        protected override async Task<CustomResponse<AddUserResult>> ExecuteCommandAsync(AddUserCommand command)
        {
            var userExists = await this.dbContext.Users
                .AnyAsync(u => u.Email == command.Email);

            if(userExists)
            {
                return CustomHttpResult.BadRequest<AddUserResult>("User with the same email already exists.");
            }

            var user = new User
            {
                Name = command.Name,
                Email = command.Email,
                Password = command.Password.ComputeSHA256Hash(),
            };

            this.dbContext.Users.Add(user);
            await this.dbContext.SaveChangesAsync();

            return CustomHttpResult.Ok(new AddUserResult(user.Id));
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
