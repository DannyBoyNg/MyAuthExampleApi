using System.Net;

namespace Services.EmailServ
{
    public class EmailSettings
    {
        /// <summary>
        ///  The domain name you wish to use for sending mails. For example: mails will be sent from no-reply@mydomain.net
        /// </summary>
        public string Domain { get; set; } = "mydomain.net";
        /// <summary>
        /// The Hostname of the mailserver you wish to use.
        /// </summary>
        public string Host { get; set; }
        /// <summary>
        /// The port number of the mailserver you wish to use. If not set
        /// </summary>
        public int? Port { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ICredentialsByHost Credentials { get; set; }
        public bool? IsBodyHtml { get; set; }
        public bool? EnableSsl { get; set; }
    }
}
