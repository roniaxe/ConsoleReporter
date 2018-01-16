using System;
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

            if (args != null && args.Length >= 2)
            {
                _dateDto = GetDatesFromCommandLineExecutable(args[0], args[1]);
            }
            Console.WriteLine($@"From Date: {_dateDto?.FromDate}, To Date: {_dateDto?.ToDate}");
            var reportLocation = ReportManager.ExportDataSet(await QueryManager.GetQueriesResultList(_dateDto));
            DistributionManager.SendReport(reportLocation);
            Console.ReadKey();
        }

        private static FromToDateDto GetDatesFromCommandLineExecutable(string arg1, string arg2)
        {
            FromToDateDto dateDto;
            try
            {
                dateDto = new FromToDateDto
                {
                    FromDate = DateTime.Parse(arg1),
                    ToDate = DateTime.Parse(arg2)
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }

            return dateDto;
        }
    }
}
