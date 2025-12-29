using FluentValidation.Results;
using NexOrder.UserService.Shared.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexOrder.UserService.Application.Common
{
    public static class Extensions
    {
        private static Dictionary<string, List<string>> GetValidationErrors(this List<ValidationFailure> errors)
        {
            return errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToList()
                );
        }

        public static CustomResponse<TResult> GetValidationResult<TResult>(this ValidationResult validationResult)
        {
            return CustomHttpResult.BadRequest<TResult>("One or more validation errors", validationResult.Errors.GetValidationErrors());
        }
    }
}
