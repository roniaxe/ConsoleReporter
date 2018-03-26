using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using ReporterConsole.DTOs;
using ReporterConsole.Utils;

namespace ReporterConsole.DistributionListHandler
{
    class ConfigFileDistributionList : IDistributionList
    {
		public AppSettings Configuration { get; }

	    public ConfigFileDistributionList(IOptions<AppSettings> configuration)
	    {
			Configuration = configuration.Value;
		}

		public IEnumerable<string> GetList()
        {
            return Configuration.DistributionList
	            .AsEnumerable()
				.Where(recipient => recipient != null)
                .Select(recipient => recipient.EmailAddress);
        }
    }
}
