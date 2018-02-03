using System;
using System.Linq;
using System.Threading.Tasks;
using ReporterConsole.DTOs;
using ReporterConsole.Utils;

namespace ReporterConsole
{
    class Program
    {
        private static FromToDateDto _dateDto;

        public static async Task Main(string[] args)
        {
            _dateDto = GetDatesFromCommandLineExecutable(args);
            Console.WriteLine($@"From Date: {_dateDto?.FromDate}, To Date: {_dateDto?.ToDate}");
            var reportLocation = ReportManager.ExportDataSet(await QueryManager.GetQueriesResultList(_dateDto));
            //DistributionManager.SendReport(reportLocation);
            DistributionManager.SendUsingSmtpClient(reportLocation);
            Environment.ExitCode = 0;
        }

        private static FromToDateDto GetDatesFromCommandLineExecutable(string[] args)
        {
            FromToDateDto dateDto;
            try
            {
                switch (args.Length)
                {
                    case 0:
                        dateDto = new FromToDateDto { FromDate = DateTime.Now, ToDate = DateTime.Now.AddDays(1) };
                        break;
                    case 1:
                        dateDto = new FromToDateDto
                        {
                            FromDate = DateTime.Parse(args[0]),
                            ToDate = DateTime.Parse(args[0]).AddDays(1)
                        };
                        break;
                    case 2:
                        dateDto = new FromToDateDto
                        {
                            FromDate = DateTime.Parse(args[0]),
                            ToDate = DateTime.Parse(args[1])
                        };
                        break;
                    default:
                        Environment.ExitCode = 1;
                        throw new Exception("Too Many Arguments");
                }
            }
            catch (Exception e)
            {
                Environment.ExitCode = 1;
                Console.WriteLine(e.Message);
                throw;
            }

            return dateDto;
        }
    }
}