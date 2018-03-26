using System.Collections.Generic;
using ReporterConsole.DTOs;

namespace ReporterConsole.DistributionListHandler
{
    public interface IDistributionList
    {
        IEnumerable<Recipient> GetList();
    }
}
