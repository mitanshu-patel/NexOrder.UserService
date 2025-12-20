using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexOrder.UserService.Application.ApiTypes
{
    public record AuthTokenResult(bool IsSuccess, string? Token, string? ErrorMessage);
}
