using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using WebScheduler.Database;
using WebScheduler.Interfaces;

namespace WebScheduler.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly DataContext dataContext;
        internal DbSet<T> dbSet;

        public GenericRepository(DataContext dataContext)
        {
            this.dataContext = dataContext;
            dbSet = dataContext.Set<T>();
        }

        public async Task AddAsync(T entity)
        {
            await dataContext.AddAsync(entity);
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> expression)
        {
            return await dbSet.Where(expression).ToListAsync();
        }

        public Task<T> GetAsync(Expression<Func<T, bool>> expression)
        {
            return dbSet.FirstAsync(expression);
        }

        public bool Exists(Expression<Func<T, bool>> expression)
        {
            return dbSet.Any(expression);
        }

        public void Remove(T entity)
        {
            dataContext.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            dataContext.RemoveRange(entities);
        }

        public void Update(T entity)
        {
            dataContext.Update(entity);
        }

        public void UpdateRange(IEnumerable<T> entities)
        {
            dataContext.UpdateRange(entities);
        }

        public IEnumerable<T> GetAll()
        {
            return dbSet.AsEnumerable();
        }
    }
}
