using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using ReporterConsole.Utils;

namespace ReporterConsole.DistributionListHandler
{
    class ConfigFileDistributionList : IDistributionList
    {
		public IConfigurationRoot Configuration { get; }

	    public ConfigFileDistributionList(IConfigurationRoot configuration)
	    {
			Configuration = configuration;
		}

		public IEnumerable<string> GetList()
        {
            return Configuration.GetSection("DistributionList")
	            .AsEnumerable()
				.Where(recipient => recipient.Value != null)
                .Select(recipient => recipient.Value);
        }
    }
}
