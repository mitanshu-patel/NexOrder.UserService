using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexOrder.UserService.Application.Users.AuthenticateUser
{
    public record AuthenticateUserCommand(string Email, string Password);
}
