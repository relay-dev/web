using Core.Plugins.Configuration;
using Web.Configuration;

namespace Web.Extensions
{
    public static class PluginsConfigurationExtensions
    {
        public static WebConfigurationBuilder AsWebConfiguration(this PluginConfigurationBuilder pluginConfigurationBuilder)
        {
            return new WebConfigurationBuilder(pluginConfigurationBuilder);
        }
    }
}
