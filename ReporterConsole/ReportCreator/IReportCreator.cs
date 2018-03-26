using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace ReporterConsole.ReportCreator
{
    public interface IReportCreator<T>
    {
        Task<List<DataTable>> Data { get; }
        Task<string> CreateReportAsync();
    }
}
