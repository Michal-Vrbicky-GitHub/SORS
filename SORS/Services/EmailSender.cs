using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net.Mail;

using System.Net;
using Microsoft.Extensions.Options;

namespace SORS.Services
{
    public class SmtpSettings
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public bool EnableSsl { get; set; }
        public string From { get; set; }
    }

    public interface IEmailSender
    {
        Task SendEmailAsync(string to, string subject, string body);
    }

    public class EmailSender : IEmailSender
    {
        private readonly SmtpSettings _smtpSettings;

        public EmailSender(IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                var client = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port)
                {
                    EnableSsl = _smtpSettings.EnableSsl,
                    Credentials = new NetworkCredential()
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_smtpSettings.From),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true,
                };

                mailMessage.To.Add(to);

                await client.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                LogError(to, subject, body, ex);
                //return false;
            }
        }

        private void LogError(string to, string subject, string body, Exception ex)
        {
            string logMessage = $"Time: {DateTime.Now}\nError sending email to {to}\nSubject: {subject}\nBody: {body}\nException: {ex}\n";
            string logFilePath = Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName, "log.txt");
            if (!File.Exists(logFilePath))
            {
                using (StreamWriter writer = File.CreateText(logFilePath))
                {
                    writer.WriteLine(logMessage);
                }
            }
            else
            {
                using (StreamWriter writer = File.AppendText(logFilePath))
                {
                    writer.WriteLine(logMessage);
                }
            }
        }
    }
}
