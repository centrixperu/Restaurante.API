using Autofac;
using Centrix.Encore.Common;
using Centrix.Encore.Repository.Interfaces.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Centrix.Encore.Repository.Implementations.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContext;
        private readonly AppSetting settings;
        private readonly ILifetimeScope _lifetimeScope;

        private Dictionary<Type, object> repositories;
        private bool _disposed;
        public Func<DateTime> CurrentDateTime { get; set; } = () => DateTime.Now;

        public UnitOfWork(IOptions<AppSetting> settings, IHttpContextAccessor httpContext)
        {
            this.settings = settings.Value;
            _context = new DataContext(new DbContextOptionsBuilder<DataContext>().UseSqlServer(this.settings.ConnectionStrings.DefaultConnection).Options);
            //_context = new DataContext(new DbContextOptionsBuilder<DataContext>().UseNpgsql(this.settings.ConnectionStrings.DefaultConnection).Options);
            _httpContext = httpContext;
        }

        public DbContext Get()
        {
            return _context;
        }

        public T Repository<T>() where T : class
        {
            return _lifetimeScope.Resolve<T>(new NamedParameter("context", _context));
        }

        public void SaveChanges()
        {
            TrackChanges();
            _context.SaveChanges();
        }

        private void TrackChanges()
        {
            if (_context.ChangeTracker.Entries().Any(e => e.State == EntityState.Added || e.State == EntityState.Modified))
            {
                var identity = _httpContext?.HttpContext?.User.FindFirst(Constants.Core.UserClaims.UserName)?.Value ?? Constants.Core.Audit.System;

                foreach (var entry in _context.ChangeTracker.Entries().Where(e => e.State == EntityState.Added || e.State == EntityState.Modified))
                {
                    if (entry.State == EntityState.Added)
                    {
                        if (entry.Metadata.FindProperty(Constants.Core.Audit.CreationUser) != null)
                            entry.CurrentValues[Constants.Core.Audit.CreationUser] = identity;

                        if (entry.Metadata.FindProperty(Constants.Core.Audit.CreationDate) != null)
                            entry.CurrentValues[Constants.Core.Audit.CreationDate] = CurrentDateTime();

                        if (entry.Metadata.FindProperty(Constants.Core.Audit.RowStatus) != null)
                            entry.CurrentValues[Constants.Core.Audit.RowStatus] = true;
                    }
                    if (entry.State == EntityState.Modified)
                    {
                        if (entry.Metadata.FindProperty(Constants.Core.Audit.ModificationUser) != null)
                            entry.CurrentValues[Constants.Core.Audit.ModificationUser] = identity;

                        if (entry.Metadata.FindProperty(Constants.Core.Audit.ModificationDate) != null)
                            entry.CurrentValues[Constants.Core.Audit.ModificationDate] = CurrentDateTime();
                    }
                }
            }
        }

        public DbSet<TEntity> Set<TEntity>() where TEntity : class
        {
            return _context.Set<TEntity>();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Dispose(bool disposing)
        {
            if (!_disposed)
                if (disposing)
                {
                    if (repositories != null)
                        repositories.Clear();

                    _context.Dispose();
                }

            _disposed = true;
        }
    }
}
