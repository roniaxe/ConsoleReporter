using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace ReporterConsole.Utils
{
    public static class DistributionManager
    {
        
        public static List<string> DistributionList { get; set; }

        static DistributionManager()
        {
            DistributionList = GetDistList().ToList();
        }

        private static IEnumerable<string> GetDistList()
        {
            var list = AppConfig.Configuration.GetSection("DistributionList").AsEnumerable();
            foreach (var keyValuePair in list)
            {
                if (keyValuePair.Key.Contains("EmailAddress"))
                {
                    yield return keyValuePair.Value;
                }
            }
        }

        public static void SendUsingSmtpClient(string attachment)
        {

            var body = $@"Hello, <br />
Attached the daily batches summary report for: <br /><br />
Date: <b>{Program.ReporterArgs.FromDate:D}</b><br />
Environment: <b>{Program.ReporterArgs.Environment}<b><br />
DB: <b>{Program.ReporterArgs.DbName}</b>";
            SmtpClient smtpClient = new SmtpClient("casarray.tfbf.com") {UseDefaultCredentials = true};
            MailMessage mailMessage = new MailMessage {From = new MailAddress("reporter@fbitn.com")};

            foreach (var recipient in DistributionList)
            {
                mailMessage.To.Add(recipient);
            }
            System.Net.Mime.ContentType contentType = new System.Net.Mime.ContentType
            {
                MediaType = System.Net.Mime.MediaTypeNames.Application.Octet,
                Name = Path.GetFileName(attachment)
            };
            mailMessage.Attachments.Add(new Attachment(attachment, contentType));
            mailMessage.Body=body;
            mailMessage.IsBodyHtml = true;
            mailMessage.Subject=$@"Daily Report - Production, alis_db_prod - {DateTime.Today:D}";
            smtpClient.Send(mailMessage);

        }
    }
}
