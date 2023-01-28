#nullable disable

using WebScheduler.Database;
using WebScheduler.Interfaces;
using WebScheduler.Models;

namespace WebScheduler.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        internal DataContext dataContext;
        private UserRepository userRepository;
        private CountryRepository countryRepository;
        private AppointmentRepository appointmentRepository;

        public UnitOfWork(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public UserRepository UserRepository
        {
            get
            {
                userRepository ??=  new UserRepository(dataContext);
                return userRepository;
            }
        }

        public CountryRepository CountryRepository
        {
            get
            {
                countryRepository ??= new CountryRepository(dataContext);
                return countryRepository;
            }
        }

        public AppointmentRepository AppointmentRepository
        {
            get
            {
                appointmentRepository ??= new AppointmentRepository(dataContext);
                return appointmentRepository;
            }
        }

        public async Task SaveAsync()
        {
            await dataContext.SaveChangesAsync();
        }

        public async Task DisposeAsync()
        {
            await dataContext.DisposeAsync();
        }
    }
}
