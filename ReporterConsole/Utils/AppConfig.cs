using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace ReporterConsole.Utils
{
    public static class AppConfig
    {
        public static IConfigurationRoot Configuration { get; set; }

        static AppConfig()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(AppContext.BaseDirectory))
                .AddJsonFile("config.json", optional: true);
            Configuration = builder.Build();
        }
    }
}
