using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace ReporterConsole.Utils
{
    class ConfigFileDistributionList : IDistributionList
    {
        public IEnumerable<string> GetList()
        {
            return AppConfig.Configuration.GetSection("DistributionList").AsEnumerable()
                .Select(recipient => recipient.Value);
        }
    }
}
