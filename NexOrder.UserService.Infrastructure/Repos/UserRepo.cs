using NexOrder.UserService.Application;
using NexOrder.UserService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexOrder.UserService.Infrastructure.Repos
{
    public class UserRepo : IUserRepo
    {
        private readonly UsersContext _context;
        public UserRepo(UsersContext context)
        {
            _context = context;
        }
        public async Task AddUserAsync(User user)
        {
            this._context.Users.Add(user);
            await this._context.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(User user)
        {
            this._context.Users.Remove(user);
            await this._context.SaveChangesAsync();
        }

        public IQueryable<User> GetUsers()
        {
            return this._context.Users.AsQueryable();
        }

        public async Task UpdateUserAsync(User user)
        {
            this._context.Entry(user).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            await this._context.SaveChangesAsync();
        }
    }
}
