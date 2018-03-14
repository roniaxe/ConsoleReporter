using System.Threading.Tasks;

namespace ReporterConsole.ReportCreator
{
    public interface IReportCreator<T>
    {
        Task<string> CreateReportAsync();
    }
}
