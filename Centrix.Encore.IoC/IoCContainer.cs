using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using EasyCaching.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;

namespace Centrix.Encore.IoC
{
    public class IoCContainer
    {
        private static IContainer container;
        private static IConfiguration configurations;
        private static IServiceCollection services;
        protected static readonly Lazy<IoCContainer> instance = new Lazy<IoCContainer>(() => new IoCContainer(), true);
        public static IoCContainer Current
        {
            get { return instance.Value; }
        }

        public static void SetServices(IServiceCollection servs, IConfiguration conf)
        {
            services = servs;
            configurations = conf;
        }

        public static void Initialize(ContainerBuilder builder)
        {

            try
            {
                var assemblies = new List<Assembly>();
                var dependencies = DependencyContext.Default.RuntimeLibraries;
                foreach (var library in dependencies)
                {
                    if (library.Name.StartsWith("Ferreyros"))
                    {
                        var assembly = Assembly.Load(new AssemblyName(library.Name));
                        assemblies.Add(assembly);
                    }
                }

                var assembliesArray = assemblies.ToArray();
                builder = new ContainerBuilder();
                builder.Populate(services);

                builder.RegisterAssemblyTypes(assembliesArray).Where(t => t.Name.EndsWith("Repository")).AsImplementedInterfaces().InstancePerDependency();
                builder.RegisterAssemblyTypes(assembliesArray).Where(t => t.Name.EndsWith("Application")).AsImplementedInterfaces().InstancePerDependency();
                builder.RegisterAssemblyTypes(assembliesArray).Where(t => t.Name.EndsWith("Service")).AsImplementedInterfaces().InstancePerDependency();

                if (configurations.GetSection("AppSettings:GoogleResources:MemoryCache:Enable").Get<bool>())
                    builder.AddAspectCoreInterceptor(x => x.CacheProviderName = EasyCachingConstValue.DefaultRedisName);

                container = builder.Build();
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException(ex.Message, ex);
            }
        }

        public T Resolve<T>()
        {
            return container.Resolve<T>();
        }
        public T Resolve<T>(string name, object value)
        {
            return container.Resolve<T>(new NamedParameter(name, value));
        }

    }
}
