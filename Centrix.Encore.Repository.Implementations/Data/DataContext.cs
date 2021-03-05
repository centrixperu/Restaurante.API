using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Centrix.Encore.Repository.Implementations.Data
{
    public class DataContext: DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            //builder.ApplyConfiguration(new ArchivoConfiguration(builder));
        }
        //public DbSet<ArchivoEntity> Archivo { get; set; }
    }
}
