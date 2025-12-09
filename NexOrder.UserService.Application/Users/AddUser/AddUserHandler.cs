using NexOrder.UserService.Application.Common;
using NexOrder.UserService.Shared.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexOrder.UserService.Application.Users.AddUser
{
    public class AddUserHandler : IHandler<AddUserCommand, CustomResponse<AddUserResult>>
    {
        public Task<CustomResponse<AddUserResult>> Handle(AddUserCommand command)
        {
            return Task.FromResult(CustomHttpResult.Ok(new AddUserResult()));
        }
    }
}
