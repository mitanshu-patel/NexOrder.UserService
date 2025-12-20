using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NexOrder.UserService.Application.Common;
using NexOrder.UserService.Application.Users.SearchUsers.DTOs;
using NexOrder.UserService.Application.Users.UpdateUser;
using NexOrder.UserService.Domain;
using NexOrder.UserService.Shared.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexOrder.UserService.Application.Users.SearchUsers
{
    public class SearchUsersHandler : RequestHandlerBase<SearchUsersQuery, CustomResponse<SearchUsersResult>>
    {
        private readonly ILogger<SearchUsersHandler> logger;
        private readonly IUserRepo userRepo;

        public SearchUsersHandler(ILogger<SearchUsersHandler> logger, IUserRepo userRepo)
        {
            this.logger = logger;
            this.userRepo = userRepo;
        }
        protected async override Task<CustomResponse<SearchUsersResult>> ExecuteCommandAsync(SearchUsersQuery command)
        {
            try
            {
                this.logger.LogInformation("SearchUsersHandler: ExecuteCommandAsync execution started");
                var users = this.userRepo.GetUsers();

                if (!string.IsNullOrEmpty(command.SearchText))
                {
                    users = users.Where(v=>v.Email.Contains(command.SearchText) || v.Name.Contains(command.SearchText));
                }

                var totalRecords = await users.CountAsync();

                var usersList = await users
                                .OrderByDescending(v=>v.CreatedAtUtc)
                                .Select(v=> new SearchUsersDto
                                {
                                    Email = v.Email,
                                    Name = v.Name,
                                    Id = v.Id
                                })
                                .Skip(command.PageIndex * command.PageSize)
                                .Take(command.PageSize)
                                .ToListAsync();

                this.logger.LogInformation("SearchUsersHandler: ExecuteCommandAsync execution completed and found {count} users", totalRecords);

                return CustomHttpResult.Ok(new SearchUsersResult(usersList, totalRecords));
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "SearchUsersHandler: exception occurred with message:{message}", ex.Message);
                throw;
            }
        }

        protected override CustomResponse<SearchUsersResult> GetValidationFailedResult(ValidationResult validationResult)
        {
            return validationResult.GetValidationResult<SearchUsersResult>();
        }

        protected override IValidator<SearchUsersQuery> GetValidator()
        {
            var validator = new InlineValidator<SearchUsersQuery>();
            validator.RuleFor(v => v.PageSize).GreaterThanOrEqualTo(1);
            validator.RuleFor(v => v.PageIndex).GreaterThanOrEqualTo(0);
            return validator;
        }
    }
}
