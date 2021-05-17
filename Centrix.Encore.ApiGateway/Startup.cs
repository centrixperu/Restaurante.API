using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Centrix.Encore.Common;
using Centrix.Encore.Common.Schema;
using Centrix.Encore.Extensions;
using Centrix.Encore.IoC;
using Centrix.Encore.Repository.Implementations.Data;
using Centrix.Encore.Repository.Implementations.Data.Base;
using Centrix.Encore.Repository.Interfaces.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Centrix.Encore.ApiGateway
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env)
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            string appSettingName = string.Empty;
            if (environmentName == "Production")
                appSettingName = ".Production";

            var builder = new ConfigurationBuilder();
            builder.SetBasePath(env.ContentRootPath)
               .AddJsonFile($"appsettings{appSettingName}.json", optional: true, reloadOnChange: true)
               .AddJsonFile($"configuration{appSettingName}.json", optional: false, reloadOnChange: true)
               .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSetting>(appSettingsSection);
            services.AddSingleton(cfg => cfg.GetService<IOptions<AppSetting>>().Value);

            services.AddAppSettingExtesion(Configuration);
            services.AddSwaggerExtesion("Seguridad API");

            var appSettings = appSettingsSection.Get<AppSetting>();
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(appSettings.JWTConfigurations.Secret));

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                ValidateIssuer = true,
                ValidIssuer = appSettings.JWTConfigurations.Iss,
                ValidateAudience = true,
                ValidAudience = appSettings.JWTConfigurations.Aud,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                RequireExpirationTime = true,
            };

            services.AddAuthentication()
                .AddJwtBearer(AuthenticateScheme.Pokemon, x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.TokenValidationParameters = tokenValidationParameters;
                });
            services.AddAuthentication()
                .AddJwtBearer(AuthenticateScheme.Seguridad, x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.TokenValidationParameters = tokenValidationParameters;
                });

            services.AddOcelot(Configuration);
            services.AddSwaggerForOcelot(Configuration);
            services.AddControllers();

            services.AddTransient(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSwaggerGen(cfg =>
            {
                cfg.DocumentFilter<HideOcelotControllersFilter>();
            });
            IoCContainer.SetServices(services, Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();

            app.UseCors(x => x
             .AllowAnyOrigin()
             .AllowAnyMethod()
             .AllowAnyHeader()
            );

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers(); ;
            });
            app.UseSwaggerForOcelotUI(c =>
            {
                c.PathToSwaggerGenerator = "/swagger/docs";
                c.InjectStylesheet("/swagger/header.css");
                c.DocumentTitle = "TALLERES";
            });
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "V1");
                c.InjectStylesheet("/swagger/header.css");
                c.DocumentTitle = "Seguridad";
            });

            await app.UseOcelot();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new ApplicationModule());
        }
        public class ApplicationModule : Module
        {
            protected override void Load(ContainerBuilder builder)
            {
                builder.RegisterInstance(new AutofacServiceProviderFactory());

                IoCContainer.Initialize(builder);
            }
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
    }
}
