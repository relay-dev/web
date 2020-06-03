using AutoMapper;
using Core.Caching;
using Core.Data;
//using Core.Plugins.AutoMapper.Data.Resolvers.DatabaseResolver;
using Core.Plugins.Microsoft.Azure.Storage;
using Core.Plugins.Microsoft.Azure.Storage.Impl;
using Core.Plugins.Microsoft.Azure.Wrappers;
using Core.Plugins.Providers;
using Core.Plugins.SQLServer.Wrappers;
using Core.Providers;
using Microservices.Caching;
using Microservices.Serialization;
using Microservices.Serialization.Impl;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Microservices.AzureFunctions.Bootstrap
{
    public class AzureFunctionsBootstrapper
    {
        protected readonly AzureFunctionsConfiguration _azureFunctionsConfiguration;

        public AzureFunctionsBootstrapper(AzureFunctionsConfiguration azureFunctionsConfiguration)
        {
            _azureFunctionsConfiguration = azureFunctionsConfiguration;
        }

        public IFunctionsHostBuilder Configure(IFunctionsHostBuilder builder)
        {
            var services = builder.Services;

            services
                .AddAutoMapper(cfg =>
                {
                    services.AddSingleton(provider =>
                    {
                        cfg.ConstructServicesUsing(type => ActivatorUtilities.CreateInstance(provider, type));
                        return cfg;
                    });
                }, _azureFunctionsConfiguration.AssembliesToScan)
                .AddDistributedMemoryCache();

            //services.AddScoped(typeof(LookupDataKeyResolver<>));
            //services.AddScoped(typeof(LookupDataValueResolver<>));
            services.AddScoped<ICacheHelper, DistributedCacheHelper>();
            services.AddScoped<IStorageAccountFactory, AzureStorageAccountFactory>();
            services.AddScoped<IConnectionStringProvider, AzureConnectionStringByConfigurationProvider>();
            services.AddTransient<IntegrationTestConnectionStringProvider>();
            services.AddTransient<IJsonSerializer, SystemJsonSerializer>();
            services.AddSingleton<Warmup.Warmup>();
            services.AddSingleton<IApplicationContextProvider>(sp => new ApplicationContextProvider(_azureFunctionsConfiguration.ApplicationContext));

            return builder;
        }
    }
}
