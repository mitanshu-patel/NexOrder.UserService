using NexOrder.UserService.Application.ApiTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexOrder.UserService.Application.Services
{
    public interface IAuthServiceClient
    {
        public Task<AuthTokenResult> GenerateTokenAsync(string username);
    }
}
