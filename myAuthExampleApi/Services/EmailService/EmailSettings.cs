using System.Net;

namespace Services.EmailService
{
    public class EmailSettings
    {
        public string Domain { get; set; }
        public string Host { get; set; }
        public int? Port { get; set; }
        public ICredentialsByHost Credentials { get; set; }
        public bool? IsBodyHtml { get; set; }
        public bool? EnableSsl { get; set; }
    }
}
