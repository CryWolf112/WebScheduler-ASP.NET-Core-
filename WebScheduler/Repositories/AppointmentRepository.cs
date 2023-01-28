using WebScheduler.Database;
using WebScheduler.Models;

namespace WebScheduler.Repositories
{
    public class AppointmentRepository : GenericRepository<Appointment>
    {
        internal DataContext dataContext;

        public AppointmentRepository(DataContext dataContext) : base(dataContext)
        {
            this.dataContext = dataContext;
        }
    }
}
