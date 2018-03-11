using System;
using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ReporterConsole.DTOs;
using ReporterConsole.Utils;

namespace ReporterConsole
{
    class Program
    {

        public static IServiceProvider SProvider;

        public static ArgsDto ReporterArgs = new ArgsDto();

        public static async Task Main(string[] args)
        {
            // Handle Arguments
            var app = new CommandLineApplication();
            InitCommandLineApp(app);
            app.Execute(args);

            // Help Switch
            if (args.Length == 1 && args[0] == "-?") return;

            // Encrypt
            if (!string.IsNullOrEmpty(ReporterArgs.TextToEncrypt))
            {
                Console.WriteLine(Encrypter.EncryptString(ReporterArgs.TextToEncrypt, Encrypter.Key));
                return;
            }

            // Init Services
            IServiceCollection services = new ServiceCollection();
            new Startup().ConfigureServices(services);

            // Get Service Provider
            SProvider = services.BuildServiceProvider();

            // Create Logger
            var logger = SProvider
                .GetService<ILoggerFactory>()
                .CreateLogger<Program>();

            // Check Date
            if (DateTime.Today > new DateTime(2018, 04, 10))
            {
                logger.LogError("Error Loading Settings..");
                Console.WriteLine("Error Loading Settings..");
                Environment.ExitCode = -1;
                return;
            }

            Console.WriteLine($@"From Date: {ReporterArgs.FromDate}, To Date: {ReporterArgs.ToDate}");

            // Create Report, Save Report Location
            var reportLocation = ReportManager.ExportDataSet(await QueryManager.GetQueriesResultList());

            // Send Email Report To Dist List
            DistributionManager.SendUsingSmtpClient(reportLocation);

            Environment.ExitCode = 0;
        }

        private static void InitCommandLineApp(CommandLineApplication app)
        {
            app.Name = "ReporterConsole";
            app.HelpOption("-?");


            app.OnExecute(() =>
            {
                Console.WriteLine("Parsing Commands...");
                return 0;
            });

            app.Command("create", command =>
            {

                command.Description = "Create The Report";
                command.HelpOption("-?");

                var fromDateOpt = command.Option("-f",
                    "From date YYYY-MM-DD",
                    CommandOptionType.SingleValue);
                var toDateOpt = command.Option("-t",
                    "To date YYYY-MM-DD",
                    CommandOptionType.SingleValue);
                var envNameOpt = command.Option("-e",
                    "To date YYYY-MM-DD",
                    CommandOptionType.SingleValue);

                command.OnExecute(() =>
                {
                    var fromDateParseResult = DateTime.TryParse(fromDateOpt.Value(), out var fromDate);
                    ReporterArgs.FromDate = fromDateParseResult ? fromDate : DateTime.Today;

                    var toDateParseResult = DateTime.TryParse(toDateOpt.Value(), out var toDate);
                    ReporterArgs.ToDate = toDateParseResult ? toDate : DateTime.Today.AddDays(1);

                    if (envNameOpt.HasValue())
                        ReporterArgs.Environment = envNameOpt.Value();
                    return 0;
                });
            });

            app.Command("encrypt", command =>
            {

                command.Description = "Encrypt Connection String";
                command.HelpOption("-?");

                var encryptString = command.Argument("[encrypt]",
                    "Text to encrypt");


                command.OnExecute(() =>
                {
                    ReporterArgs.TextToEncrypt = encryptString.Value;
                    return 0;
                });
            });
        }
    }
}