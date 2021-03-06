﻿namespace Web.Rest.Configuration
{
    public class SwaggerConfiguration
    {
        private string _name;

        public string Title { get; set; }
        public int MajorVersion { get; set; }
        public int MinorVersion { get; set; }
        public string Description { get; set; }
        public string Version => $"{MajorVersion}.{MinorVersion}";

        public string Name
        {
            get => (_name ??= Title).Replace(" ", string.Empty);
            set => _name = value;
        }
    }
}
