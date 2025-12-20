using NexOrder.UserService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexOrder.UserService.Application
{
    public interface IUserRepo
    {
        public IQueryable<User> GetUsers();

        public Task AddUserAsync(User user);

        public Task UpdateUserAsync(User user);

        public Task DeleteUserAsync(User user);
    }
}
