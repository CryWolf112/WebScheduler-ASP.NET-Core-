using WebScheduler.Repositories;

namespace WebScheduler.Interfaces
{
    public interface IUnitOfWork
    {
        UserRepository UserRepository { get; }

        CountryRepository CountryRepository { get; }

        AppointmentRepository AppointmentRepository { get; }

        Task SaveAsync();

        Task DisposeAsync();
    }
}
