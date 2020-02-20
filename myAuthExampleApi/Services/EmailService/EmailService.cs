using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;

namespace Services.EmailService
{
    public class EmailService : IEmailService
    {
        public EmailSettings Settings { get; set; }

        public EmailService(IOptions<EmailSettings> settings) => Settings = settings.Value;

        public async void SendEmail(MailAddress from, IEnumerable<string> to, string subject, string body, IEnumerable<File> files = null, IEnumerable<string> cc = null, IEnumerable<string> bcc = null, MailPriority? priority = null)
        {
            if (Settings.Domain == null || Settings.Host == null) throw new System.Exception("No mailsettings found. Please contact site admin.");
            using var smtpClient = new SmtpClient();
            using var message = new MailMessage();

            smtpClient.Host = Settings.Host;
            if (Settings.Port != null) smtpClient.Port = Settings.Port.Value;
            if (Settings.EnableSsl != null) smtpClient.EnableSsl = Settings.EnableSsl.Value;
            if (Settings.Credentials != null)
            {
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = Settings.Credentials;
            }

            message.From = from;
            message.To.Add(string.Join(",", to));
            message.Subject = subject;
            message.Body = body;
            if (bcc != null) message.Bcc.Add(string.Join(",", bcc));
            if (cc != null) message.CC.Add(string.Join(",", cc));
            if (priority != null) message.Priority = priority.Value;
            if (Settings.IsBodyHtml != null) message.IsBodyHtml = Settings.IsBodyHtml.Value;

            if (files != null)
            {
                foreach (var file in files)
                {
                    var stream = new MemoryStream(file.FileData);
                    var attachment = new Attachment(stream, file.FileName);
                    message.Attachments.Add(attachment);
                }
            }

            await smtpClient.SendMailAsync(message);
        }

        public bool IsEmailValid(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
