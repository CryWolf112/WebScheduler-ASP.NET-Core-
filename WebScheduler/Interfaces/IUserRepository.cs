using WebScheduler.Models;

namespace WebScheduler.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        public IEnumerable<User> GetAllUsersInRole(string roleId);
    }
}
