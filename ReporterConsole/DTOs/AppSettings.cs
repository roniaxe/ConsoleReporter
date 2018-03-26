using System;
using System.Collections.Generic;
using System.Text;

namespace ReporterConsole.DTOs
{
    public class AppSettings
    {
        public List<Recipient> DistributionList { get; set; }
        public string SmtpClientAddress { get; set; }
        public string SenderMailAddress { get; set; }
        public string ReportsLocation { get; set; }
        public bool? SendReport { get; set; }
    }
}
