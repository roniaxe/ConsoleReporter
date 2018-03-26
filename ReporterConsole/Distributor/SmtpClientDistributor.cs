using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ReporterConsole.DistributionListHandler;
using ReporterConsole.DTOs;

namespace ReporterConsole.Distributor
{
    class SmtpClientDistributor : IDistributor
    {
        private readonly ILogger<SmtpClientDistributor> _logger;

        public IEnumerable<Recipient> DistributionList { get; }
        public AppSettings Configuration { get; }
        public string Attachment { get; set; }

        public SmtpClientDistributor(
            IDistributionList distributionList,
            ILogger<SmtpClientDistributor> logger, 
            IOptions<AppSettings> configuration)
        {
            DistributionList = distributionList.GetList();
            Configuration = configuration.Value;
            _logger = logger;
        }

        public void Execute()
        {
            var smtpClientAddress = Configuration.SmtpClientAddress;
            var senderMailAddress = Configuration.SenderMailAddress;

            SmtpClient smtpClient = new SmtpClient(smtpClientAddress) { UseDefaultCredentials = true };
            var mailMessage = ConfigMessage(senderMailAddress);

            _logger.LogInformation("Sending Report To Distribution List");
            smtpClient.Send(mailMessage);
        }

        private MailMessage ConfigMessage(string senderMailAddress)
        {
            var body = CreateHtmlBody();
            MailMessage mailMessage = new MailMessage { From = new MailAddress(senderMailAddress) };

            foreach (var recipient in DistributionList)
            {
                mailMessage.To.Add(recipient.EmailAddress);
            }

            System.Net.Mime.ContentType contentType = new System.Net.Mime.ContentType
            {
                MediaType = System.Net.Mime.MediaTypeNames.Application.Octet,
                Name = Path.GetFileName(Attachment)
            };
            mailMessage.Attachments.Add(new Attachment(Attachment, contentType));
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
	<h4>Hello,</br> Attached the report for <b>{Program.ReporterArgs.FromDate:D}</b></h4>
	<p><b>Environment:</b> {Program.ReporterArgs.Environment}</p>
	<p><b>Database:</b> {Program.ReporterArgs.DbName}</p>
</body>
</html> ";

            return html;
        }
    }
}
