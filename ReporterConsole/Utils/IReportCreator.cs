using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ReporterConsole.Utils
{
    public interface IReportCreator<T>
    {
        Task<string> CreateReportAsync();
    }
}
