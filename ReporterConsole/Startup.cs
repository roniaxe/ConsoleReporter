using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ReporterConsole.Data;
using ReporterConsole.DAOs;
using ReporterConsole.Utils;

namespace ReporterConsole
{
    public class Startup
    {
        private IConfigurationRoot Configuration { get; }
        private string ConnectionString { get; }

        public Startup()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(AppContext.BaseDirectory))
                .AddJsonFile("config.json", optional: true);
            Configuration = builder.Build();

            var env = string.IsNullOrEmpty(Program.ReporterArgs.Environment)
                ? "Production"
                : Program.ReporterArgs.Environment;

            var encryptedConnectionString = Configuration.GetConnectionString(env);
            if (string.IsNullOrEmpty(Program.ReporterArgs.TextToEncrypt) && string.IsNullOrEmpty(encryptedConnectionString))
            {
                Environment.ExitCode = -1;
                throw new Exception($"Connection String Is Empty For {env}. Add It To config.json");
            }

            ConnectionString = Encrypter.DecryptString(encryptedConnectionString, Encrypter.Key);
            Program.ReporterArgs.DbName = new SqlConnectionStringBuilder(ConnectionString).InitialCatalog;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IDistributionList, ConfigFileDistributionList>();
            services.AddTransient<IDistributor, SmtpClientDistributor>();
            services.AddTransient<IReportCreator<DataTable>, ExcelReportCreator>();
            services.AddLogging(builder => builder.AddConsole().AddDebug());
            services.AddSingleton<IConfigurationRoot>(Configuration);
            services.AddDbContext<AlisUatContext>(builder => builder.UseSqlServer(ConnectionString));
            services.AddScoped<IRepository, BatchAuditRepo>();
        }
    }
}
