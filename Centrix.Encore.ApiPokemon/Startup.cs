using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Centrix.Encore.ApiPokemon.Middlewares;
using Centrix.Encore.Common;
using Centrix.Encore.Common.Schema;
using Centrix.Encore.Extensions;
using Centrix.Encore.Repository.Implementations.Data;
using Centrix.Encore.Repository.Implementations.Data.Base;
using Centrix.Encore.Repository.Interfaces.Data;
using Centrix.Encore.Service.Implementations.Base;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Centrix.Encore.ApiPokemon
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IServiceCollection services { get; private set; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAppSettingExtension(Configuration);
            services.AddJWTExtesion(Configuration, AuthenticateScheme.Pokemon);
            services.AddSwaggerExtesion("Pokemon API");
            services.AddControllers()
                .AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );

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

            var appSettingsSection = Configuration.GetSection("AppSettings");
            var appSettings = appSettingsSection.Get<AppSetting>();
            this.services = services;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseMiddleware<GCMiddleware>();

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
                c.DocumentTitle = "Pokemon";
            });
        }
        public void ConfigureContainer(ContainerBuilder builder)
        {
            var assemblies = new List<Assembly>();
            var dependencies = DependencyContext.Default.RuntimeLibraries;
            foreach (var library in dependencies)
            {
                if (library.Name.StartsWith("Centrix"))
                {
                    var assembly = Assembly.Load(new AssemblyName(library.Name));
                    assemblies.Add(assembly);
                }
            }

            var assembliesArray = assemblies.ToArray();

            //builder.Populate(services);
            builder.RegisterAssemblyTypes(assembliesArray).Where(t => t.Name.EndsWith("Service")).AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterAssemblyTypes(assembliesArray).Where(t => t.Name.EndsWith("Repository")).AsImplementedInterfaces().InstancePerDependency();
            builder.RegisterAssemblyTypes(assembliesArray).Where(t => t.Name.EndsWith("Application")).AsImplementedInterfaces().InstancePerLifetimeScope();

            //IEnumerable parameters = new List<Autofac.Core.Parameter>();
            Autofac.IContainer container = null;
            builder.Register(c => container).AsSelf().SingleInstance();

            builder.RegisterGeneric(typeof(BaseRepository<>)).As(typeof(IBaseRepository<>)).InstancePerDependency();
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerLifetimeScope();
            builder.RegisterType<HttpContextAccessor>().As<IHttpContextAccessor>().SingleInstance();

        }


        public class HideOcelotControllersFilter : IDocumentFilter
        {
            private static readonly string[] _ignoredPaths = { "/configuration", "/outputcache/{region}" };

            public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
            {
                foreach (var ignorePath in _ignoredPaths)
                {
                    swaggerDoc.Paths.Remove(ignorePath);
                }
            }

        }
        public void InitializeDataBase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                try
                {
                    var context = serviceScope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                    //DBInitilizer.Initialize(context).Wait();
                }
                catch (Exception ex)
                {
                    //TODO: Implementar LOG
                }
            }
        }
    }
}
