using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Autofac;
using Centrix.Encore.Common.Schema;
using Centrix.Encore.Extensions;
using Centrix.Encore.IoC;
using Centrix.Encore.Repository.Implementations.Data;
using Centrix.Encore.Repository.Implementations.Data.Base;
using Centrix.Encore.Repository.Interfaces.Data;
using Centrix.Encore.Service.Implementations.Base;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Centrix.Encore.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddControllersWithViews();
            services.AddAppSettingExtesion(Configuration);
            services.AddJWTExtesion(Configuration, AuthenticateScheme.Seguridad);
            services.AddSwaggerExtesion("Encore API");

            services.AddControllers()
                .AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );

            services.AddTransient(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddHttpClient<HttpClientService>().ConfigureHttpMessageHandlerBuilder(builder =>
            {
                builder.PrimaryHandler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (m, c, ch, e) => true
                };
            });

            services.AddSwaggerGen(cfg =>
            {
                cfg.CustomSchemaIds(type => type.ToString());
                cfg.DocumentFilter<HideOcelotControllersFilter>();
            });

            //services.AddLoggingException(Configuration);

            string projectName = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;

            //services.AddSingleton<ILoggerApplication, Logger>((provider) => { return new Logger(System.Reflection.Assembly.GetEntryAssembly().GetName().Name); });

            IoCContainer.SetServices(services, Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors(x => x
             .AllowAnyOrigin()
             .AllowAnyMethod()
             .AllowAnyHeader()
            );

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseStaticFiles();
            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "V1");
                c.InjectStylesheet("/swagger/header.css");
                c.DocumentTitle = "Encore";
            });
        }
        public void ConfigureContainer(ContainerBuilder builder)
        {

            builder.RegisterModule(new ApplicationModule());
        }
        public class ApplicationModule : Module
        {
            protected override void Load(ContainerBuilder builder)
            {
                IoCContainer.Initialize(builder);
            }
        }

    }
}
