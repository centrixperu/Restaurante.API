using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Centrix.Encore.Repository.Interfaces.Data
{
    public interface IUnitOfWork : IDisposable
    {
        void Dispose();
        void SaveChanges();
        void Dispose(bool disposing);
        T Repository<T>() where T : class;
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        DbContext Get();
    }
}
