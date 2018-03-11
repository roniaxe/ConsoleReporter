using System;
using System.Collections.Generic;
using System.Text;

namespace ReporterConsole.Utils
{
    public interface IDistributionList
    {
        IEnumerable<string> GetList();
    }
}
