namespace Microservices.Configuration
{
    public class SwaggerConfiguration
    {
        public string Title { get; set; }
        public int MajorVersion { get; set; }
        public int MinorVersion { get; set; }
        public string Description { get; set; }
        public string Version => $"{MajorVersion}.{MinorVersion}";
    }
}
