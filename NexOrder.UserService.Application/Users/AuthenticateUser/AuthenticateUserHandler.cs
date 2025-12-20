using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NexOrder.UserService.Application.Common;
using NexOrder.UserService.Application.Services;
using NexOrder.UserService.Shared.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexOrder.UserService.Application.Users.AuthenticateUser
{
    public class AuthenticateUserHandler : RequestHandlerBase<AuthenticateUserCommand, CustomResponse<AuthenticateUserResult>>
    {
        private readonly IUserRepo userRepo;
        private readonly IAuthServiceClient authServiceClient;
        private readonly ILogger<AuthenticateUserHandler> logger;

        public AuthenticateUserHandler(ILogger<AuthenticateUserHandler> logger, IAuthServiceClient authServiceClient, IUserRepo userRepo)
        {
            this.logger = logger;
            this.userRepo = userRepo;
            this.authServiceClient = authServiceClient;
        }
        protected override async Task<CustomResponse<AuthenticateUserResult>> ExecuteCommandAsync(AuthenticateUserCommand command)
        {
            try
            {
                this.logger.LogDebug("Authenticating user with email: {Email}", command.Email);
                var user = await this.userRepo.GetUsers().Where(v => v.Email == command.Email).Select(v=> new {v.Password}).FirstOrDefaultAsync();
                
                if (user == null)
                { 
                    this.logger.LogError("User with email {Email} not found.", command.Email);
                    return CustomHttpResult.NotFound<AuthenticateUserResult>($"User with email {command.Email} not found.");
                }

                var encryptedPassword = command.Password.ComputeSHA256Hash();
                if (encryptedPassword.Equals(user.Password))
                {
                    var result = await this.authServiceClient.GenerateTokenAsync(command.Email);
                    if(result.IsSuccess && result.Token != null)
                    {
                        this.logger.LogInformation("User with email {Email} authenticated successfully.", command.Email);
                        return CustomHttpResult.Ok(new AuthenticateUserResult(result.Token));
                    }
                    else
                    {
                        throw new Exception(result.ErrorMessage);
                    }
                }

                return CustomHttpResult.UnAuthorized<AuthenticateUserResult>("Incorrect email or password");
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error occurred while authenticating user with email {Email} with message :{message}", command.Email, ex.Message);
                throw;
            }
        }

        protected override CustomResponse<AuthenticateUserResult> GetValidationFailedResult(ValidationResult validationResult)
        {
            return validationResult.GetValidationResult<AuthenticateUserResult>();
        }

        protected override IValidator<AuthenticateUserCommand> GetValidator()
        {
           var validator = new InlineValidator<AuthenticateUserCommand>();
            validator.RuleFor(x => x.Email).NotEmpty();
            validator.RuleFor(x => x.Password).NotEmpty();
            return validator;
        }
    }
}
