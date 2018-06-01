using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ReporterConsole.Data;
using ReporterConsole.DAOs;
using ReporterConsole.DistributionListHandler;
using ReporterConsole.Distributor;
using ReporterConsole.DTOs;
using ReporterConsole.ReportCreator;
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
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("config.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true);
            Configuration = builder.Build();

            var env = string.IsNullOrEmpty(Program.ReporterArgs.Environment)
                ? "Production"
                : Program.ReporterArgs.Environment;

            var encryptedConnectionString = Configuration.GetConnectionString(env);
            if (string.IsNullOrEmpty(encryptedConnectionString))
            {
                Environment.ExitCode = -1;
                throw new Exception($"Connection String Is Empty For {env}. Add It To config.json");
            }

            ConnectionString = Encrypter.DecryptString(encryptedConnectionString, Encrypter.Key);
            Program.ReporterArgs.DbName = new SqlConnectionStringBuilder(ConnectionString).InitialCatalog;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AppSettings>(Configuration);
            services.AddTransient<IDistributionList, ConfigFileDistributionList>();
            services.AddTransient<IDistributor, SmtpClientDistributor>();
            //services.AddTransient<IReportCreator<DataTable>, ExcelReportCreator>();
            services.AddTransient<IReportCreator<DataTable>, NpoiReportCreator>();
            services.AddLogging(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Information);
                builder.AddConsole();
                //builder.AddDebug();
            });
            services.AddSingleton<IConfigurationRoot>(Configuration);
            services.AddDbContext<AlisUatContext>(builder =>
            {
                builder.UseSqlServer(ConnectionString);
                builder.EnableSensitiveDataLogging();
            });
            services.AddDbContext<DefectContext>(builder => builder.UseSqlite($@"Data Source={AppContext.BaseDirectory}\defects.db"));
            services.AddScoped<IBatchAuditRepository, BatchAuditRepo>();
        }
    }
}
