using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexOrder.UserService.Messages.Events
{
    public record UserUpdated(int Id, string Email, string FullName);
}
