using Centrix.Encore.Repository.Interfaces.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Centrix.Encore.Repository.Implementations.Data.Base
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private DbSet<T> table = null;
        private readonly DataContext context;

        public BaseRepository(DataContext context)
        {
            this.context = context;
            table = context.Set<T>();
        }


        public async Task<T> GetById(int id) => await table.FindAsync(id);
        public async Task<T> GetById(int? id) => await table.FindAsync(id);
        public Task<T> FirstOrDefault(Expression<Func<T, bool>> predicate)
            => table.FirstOrDefaultAsync(predicate);

        public T Add(T entity)
        {
            table.Add(entity);
            return entity;
        }

        public void Update(T entity)
        {
            table.Attach(entity);
            context.Entry(entity).State = EntityState.Modified;
        }

        public void Remove(T entity)
        {
            table.Attach(entity);
            context.Entry(entity).State = EntityState.Modified;
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await table.ToListAsync();
        }

        public async Task<IEnumerable<T>> GetWhere(Expression<Func<T, bool>> predicate)
        {
            return await table.Where(predicate).ToListAsync();
        }

        public Task<int> CountAll() => table.CountAsync();

        public Task<int> CountWhere(Expression<Func<T, bool>> predicate) => table.CountAsync(predicate);

    }
}
