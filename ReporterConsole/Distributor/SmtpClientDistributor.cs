using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ReporterConsole.DistributionListHandler;
using ReporterConsole.ReportCreator;

namespace ReporterConsole.Distributor
{
	class SmtpClientDistributor : IDistributor
	{
		private string _attachment;
		private readonly IReportCreator<DataTable> _reporter;
		private readonly ILogger<SmtpClientDistributor> _logger;

		public IEnumerable<string> DistributionList { get; }
		public IConfiguration Configuration { get; }

		public SmtpClientDistributor(IDistributionList distributionList, IReportCreator<DataTable> reporter,
			ILoggerFactory loggerFactory, IConfigurationRoot configuration)
		{
			DistributionList = distributionList.GetList();
			_reporter = reporter;
			Configuration = configuration;
			_logger = loggerFactory.CreateLogger<SmtpClientDistributor>();
		}

		public async Task ExecuteAsync()
		{
			_logger.LogInformation("Getting SMTP Client Address Information From config.json");
			var smtpClientAddress = Configuration.GetSection("SmtpClientAddress").Value;
			_logger.LogInformation("Getting Sender Mail Address From config.json");
			var senderMailAddress = Configuration.GetSection("SenderMailAddress").Value;

			SmtpClient smtpClient = new SmtpClient(smtpClientAddress) { UseDefaultCredentials = true };
			_attachment = await _reporter.CreateReportAsync();
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
	<h4>Hello,</br> Attached the report for <b>{Program.ReporterArgs.FromDate:D}</b></h4>
	<p><b>Environment:</b> {Program.ReporterArgs.Environment}</p>
	<p><b>Database:</b> {Program.ReporterArgs.DbName}</p>
</body>
</html> ";

			return html;
		}
	}
}
