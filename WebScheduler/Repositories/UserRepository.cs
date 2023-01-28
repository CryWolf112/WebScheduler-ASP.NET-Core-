using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using WebScheduler.Database;
using WebScheduler.Interfaces;
using WebScheduler.Models;

namespace WebScheduler.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        internal DataContext dataContext;

        public UserRepository(DataContext dataContext) : base(dataContext)
        {
            this.dataContext = dataContext;
        }

        public IEnumerable<User> GetAllUsersInRole(string roleId)
        {
            return dataContext.Users
                .Join(dataContext.UserRoles,
                user => user.Id,
                userRole => userRole.UserId,
                (user, userRole) => new { user, userRole })
                .Where(x => x.userRole.RoleId == roleId)
                .Select(x => x.user);
        }
    }
}
