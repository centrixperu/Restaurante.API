using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Autofac.Extensions.DependencyInjection;

namespace Centrix.Encore.ApiPokemon
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebHost.CreateDefaultBuilder(args)
                              .UseContentRoot(Directory.GetCurrentDirectory())
                              .UseStartup<Startup>()
                              .ConfigureServices(services => services.AddAutofac())
                              .Build()
                              .Run();
        }
    }
}
