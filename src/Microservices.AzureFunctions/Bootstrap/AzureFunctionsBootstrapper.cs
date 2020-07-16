using AutoMapper;
using Core.Caching;
using Core.Plugins.AutoMapper.Data.Resolvers.DatabaseResolver;
using Core.Plugins.Microsoft.Azure.Storage;
using Core.Plugins.Microsoft.Azure.Storage.Impl;
using Core.Plugins.Microsoft.Azure.Wrappers;
using Core.Plugins.Providers;
using Core.Providers;
using FluentCommander.Core.Impl;
using FluentCommander.SqlServer;
using FluentValidation.AspNetCore;
using MediatR;
using Microservices.Caching;
using Microservices.Serialization;
using Microservices.Serialization.Impl;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Microservices.AzureFunctions.Bootstrap
{
    public class AzureFunctionsBootstrapper
    {
        private readonly AzureFunctionsConfiguration _azureFunctionsConfiguration;
        
        public AzureFunctionsBootstrapper(AzureFunctionsConfiguration azureFunctionsConfiguration)
        {
            _azureFunctionsConfiguration = azureFunctionsConfiguration;
        }

        public IFunctionsHostBuilder Configure(IFunctionsHostBuilder builder)
        {
            var services = builder.Services;

            services.AddMvcCore()
                .AddFluentValidation()
                .AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

            services
                .AddAutoMapper(cfg =>
                {
                    services.AddSingleton(provider =>
                    {
                        cfg.ConstructServicesUsing(type => ActivatorUtilities.CreateInstance(provider, type));
                        return cfg;
                    });
                }, _azureFunctionsConfiguration.AssembliesToScan)
                .AddMediatR(_azureFunctionsConfiguration.AssembliesToScan)
                .AddDistributedMemoryCache();

            services
                .AddSqlServerDatabaseCommander(_azureFunctionsConfiguration.Configuration);

            services.AddScoped(typeof(LookupDataKeyResolver<>));
            services.AddScoped(typeof(LookupDataValueResolver<>));
            services.AddScoped<ICacheHelper, DistributedCacheHelper>();
            services.AddScoped<IStorageAccountFactory, AzureStorageAccountFactory>();
            services.AddScoped<IConnectionStringProvider, AzureConnectionStringByConfigurationProvider>();
            services.AddTransient<IJsonSerializer, SystemJsonSerializer>();
            services.AddSingleton<Warmup.Warmup>();
            services.AddSingleton<IApplicationContextProvider>(sp => new ApplicationContextProvider(_azureFunctionsConfiguration.ApplicationContext));

            var connectionStringCollection = new ConnectionStringCollection(_azureFunctionsConfiguration.Configuration);

            if (connectionStringCollection.ConnectionStringNames.Contains("DefaultStorageConnection"))
            {
                services.AddTransient<IStorageAccount>(sp => new AzureStorageAccount(sp.GetService<IConnectionStringProvider>().Get("DefaultStorageConnection")));
            }

            return builder;
        }
    }
}
