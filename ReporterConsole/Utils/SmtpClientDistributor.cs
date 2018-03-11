using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ReporterConsole.Utils
{
    class SmtpClientDistributor : IDistributor
    {
        private string _attachment;
        private readonly IReportCreator<DataTable> _reporter;
        private readonly ILogger<SmtpClientDistributor> _logger;

        public IEnumerable<string> DistributionList { get; }

        public SmtpClientDistributor(IDistributionList distributionList, IReportCreator<DataTable> reporter, ILoggerFactory loggerFactory)
        {
            DistributionList = distributionList.GetList();
            _reporter = reporter;
            _logger = loggerFactory.CreateLogger<SmtpClientDistributor>();
        }

        public async Task ExecuteAsync()
        {
            _logger.LogInformation("Getting SMTP Client Address Information From config.json");
            var smtpClientAddress = AppConfig.Configuration.GetSection("SmtpClientAddress").Value;
            _logger.LogInformation("Getting Sender Mail Address From config.json");
            var senderMailAddress = AppConfig.Configuration.GetSection("SenderMailAddress").Value;

            SmtpClient smtpClient = new SmtpClient(smtpClientAddress) { UseDefaultCredentials = true };
            _attachment = await _reporter.CreateReportAsync();
            var mailMessage = ConfigMessage(senderMailAddress);
            _logger.LogInformation("Sending Report To Distribution List");
            smtpClient.Send(mailMessage);
        }

        private MailMessage ConfigMessage(string senderMailAddress)
        {
            var body = CreateHtmlBody();
            MailMessage mailMessage = new MailMessage {From = new MailAddress(senderMailAddress)};

            foreach (var recipient in DistributionList)
            {
                mailMessage.To.Add(recipient);
            }

            System.Net.Mime.ContentType contentType = new System.Net.Mime.ContentType
            {
                MediaType = System.Net.Mime.MediaTypeNames.Application.Octet,
                Name = Path.GetFileName(_attachment)
            };
            mailMessage.Attachments.Add(new Attachment(_attachment, contentType));
            mailMessage.Body = body;
            mailMessage.IsBodyHtml = true;
            mailMessage.Subject =
                $@"Daily Batch Report - {Program.ReporterArgs.Environment}, {Program.ReporterArgs.DbName} - {DateTime.Today:D}";
            return mailMessage;
        }

        private string CreateHtmlBody()
        {
            var html =
                $@"<!DOCTYPE html>
<html>
<body>
	<h1>Hello, attached the report for <b>{Program.ReporterArgs.FromDate}</b></h1></br>
	<p><b>Environment:</b> {Program.ReporterArgs.Environment}</p>
	<p><b>Database:</b> {Program.ReporterArgs.DbName}</p>
</body>
</html> ";

            return html;
        }
    }
}
