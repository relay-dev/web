using Core.Plugins.Configuration;

namespace Web.Configuration
{
    public class WebConfiguration : PluginConfiguration
    {
        public WebConfiguration(bool isAddDiagnostics)
        {
            IsAddDiagnostics = isAddDiagnostics;
        }

        public WebConfiguration() : this(true) { }

        public bool IsAddDiagnostics { get; set; }
    }
}
