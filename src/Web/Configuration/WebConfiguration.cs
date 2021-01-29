using Core.Plugins.Configuration;

namespace Web.Configuration
{
    public class WebConfiguration : PluginConfiguration
    {
        public WebConfiguration(bool isAddDiagnostics, bool isEnableRetryOnDbContextFailure)
        {
            IsAddDiagnostics = isAddDiagnostics;
            IsEnableRetryOnDbContextFailure = isEnableRetryOnDbContextFailure;
        }

        public WebConfiguration() : this(true, true) { }

        public bool IsAddDiagnostics { get; set; }

        public bool IsEnableRetryOnDbContextFailure { get; set; }
    }
}
