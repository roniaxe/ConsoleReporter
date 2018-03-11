using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace ReporterConsole.Utils
{
    public interface IDistributor
    {
        Task ExecuteAsync();
    }
}
