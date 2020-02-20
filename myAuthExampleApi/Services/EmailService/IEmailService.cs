using System.Collections.Generic;
using System.Net.Mail;

namespace Services.EmailService
{
    public interface IEmailService
    {
        public EmailSettings Settings { get; set; }

        bool IsEmailValid(string email);
        void SendEmail(MailAddress from, IEnumerable<string> to, string subject, string body, IEnumerable<File> files = null, IEnumerable<string> cc = null, IEnumerable<string> bcc = null, MailPriority? priority = null);
    }
}
