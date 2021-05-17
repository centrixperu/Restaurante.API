using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Autofac.Extensions.DependencyInjection;
using System;

namespace Centrix.Encore.ApiGateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var directory = System.IO.Directory.GetCurrentDirectory();
            var conf = GetConfig();
            IWebHostBuilder builder = new WebHostBuilder();
            builder.ConfigureServices(s =>
            {
                s.AddSingleton(builder);

            });

            //.UseServiceProviderFactory(new AutofacServiceProviderFactory());
            builder.UseKestrel()

                   .UseContentRoot(Directory.GetCurrentDirectory())
                   .UseStartup<Startup>()
                   .UseUrls($"http://*:{conf.GetSection("Host:Port").Get<string>()}")
                   .ConfigureServices(services =>
                        services.AddAutofac()
                   );

            var host = builder.Build();
            host.Run();
        }

        private static IConfigurationRoot GetConfig()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            string appSettingName = string.Empty;
            if (env == "Production")
                appSettingName = ".Production";


            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings{appSettingName}.json", optional: true)
                .AddEnvironmentVariables();

            return builder.Build(); ;
        }
    }
}
