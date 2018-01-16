using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Outlook = Microsoft.Office.Interop.Outlook;

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

        public static void SendReport(string attachment)
        {
            var to = DistributionList;
            var body = $@"Hello, 
Attached the daily batches summary report for:
Date: {DateTime.Today:D}
Environment: Production
DB: alis_db_prod";
            SendEmailFromAccount(
                new Outlook.Application(),
                $@"Daily Report - Production, alis_db_prod - {
                    DateTime.Today:D}",
                to,
                body,
                "roni.axelrad@sapiens.com",
                attachment);
            Console.WriteLine(@"Mail Sent!");
        }

        public static void SendEmailFromAccount(Outlook.Application application, string subject, List<string> to, string body, string smtpAddress, string attachment)
        {

            // Create a new MailItem and set the To, Subject, and Body properties.
            Outlook.MailItem newMail = (Outlook.MailItem)application.CreateItem(Outlook.OlItemType.olMailItem);
            string toStr = to.Aggregate<string, string>(null, (current, rec) => current + rec + ";");
            newMail.To = toStr;
            newMail.Subject = subject;
            newMail.Body = body;
            newMail.Attachments.Add(attachment, Outlook.OlAttachmentType.olByValue, Type.Missing, Type.Missing);

            // Retrieve the account that has the specific SMTP address.
            Outlook.Account account = GetAccountForEmailAddress(application, smtpAddress);
            // Use this account to send the e-mail.
            newMail.SendUsingAccount = account;
            newMail.Send();
        }
        public static Outlook.Account GetAccountForEmailAddress(Outlook.Application application, string smtpAddress)
        {

            // Loop over the Accounts collection of the current Outlook session.
            Outlook.Accounts accounts = application.Session.Accounts;
            try
            {
                //foreach (Outlook.Account account in accounts)
                //{
                //    // When the e-mail address matches, return the account.
                //    if (account.SmtpAddress == smtpAddress)
                //    {
                //        return account;
                //    }
                //}

                for (int i = 1; i <= accounts.Count; i++)
                {
                    // When the e-mail address matches, return the account.
                    if (accounts[i].SmtpAddress == smtpAddress)
                    {
                        return accounts[i];
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            throw new Exception($"No Account with SmtpAddress: {smtpAddress} exists!");
        }
    }
}
