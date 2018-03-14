using System.Collections.Generic;

namespace ReporterConsole.DistributionListHandler
{
    public interface IDistributionList
    {
        IEnumerable<string> GetList();
    }
}
