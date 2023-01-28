using Microsoft.EntityFrameworkCore;
using WebScheduler.Database;
using WebScheduler.Interfaces;
using WebScheduler.Models;

namespace WebScheduler.Repositories
{
    public class CountryRepository : GenericRepository<Country>
    {
        internal DataContext dataContext;

        public CountryRepository(DataContext dataContext) : base(dataContext)
        {
            this.dataContext = dataContext;
        }
    }
}
