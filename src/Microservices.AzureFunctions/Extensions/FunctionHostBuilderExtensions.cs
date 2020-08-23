using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microservices.AzureFunctions.Extensions
{
    public static class FunctionHostBuilderExtensions
    {
        /// <summary>
        /// Source: https://stackoverflow.com/questions/59959258/how-to-add-an-appsettings-json-file-to-my-azure-function-3-0-configuration
        /// Set up the configuration for the builder itself. This replaces the 
        /// currently registered configuration with additional custom configuration.
        /// This can be called multiple times and the results will be additive.
        /// </summary>
        public static IFunctionsHostBuilder ConfigureHostConfiguration(this IFunctionsHostBuilder builder, Action<IServiceProvider, IConfigurationBuilder> configureDelegate)
        {
            var providers = new List<IConfigurationProvider>();

            // Cache all current configuration provider
            foreach (var descriptor in builder.Services.Where(d => d.ServiceType == typeof(IConfiguration)).ToList())
            {
                if (!(descriptor.ImplementationInstance is IConfigurationRoot existingConfiguration))
                {
                    continue;
                }

                providers.AddRange(existingConfiguration.Providers);

                builder.Services.Remove(descriptor);
            }

            // Add new configuration based on original and newly added configuration
            builder.Services.AddSingleton<IConfiguration>(sp =>
            {
                var configurationBuilder = new ConfigurationBuilder();

                // Call custom configuration
                configureDelegate?.Invoke(sp, configurationBuilder);
                providers.AddRange(configurationBuilder.Build().Providers);

                return new ConfigurationRoot(providers);
            });

            return builder;
        }
    }
}
