using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Box.Common.Services
{
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713
    public class SMTPEmailSender : IEmailSender
    {
        private readonly SMTPSettings _settings;

        public SMTPEmailSender(IOptions<SMTPSettings> settings)
        {
            _settings = settings.Value;
        }

        public Task SendEmailAsync(string to, string subject, string message)
        {
            return SendEmailAsync(_settings.DefaultSenderAccount, to, subject, message);
        }

        public Task SendEmailAsync(string from, string to, string subject, string message)
        {
            System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage(from, to);
            msg.Subject = subject;
            msg.Body = message;
            msg.IsBodyHtml = true;

            var smtp = ConfigureSMTPClient();
            return smtp.SendMailAsync(msg);
        }

        private System.Net.Mail.SmtpClient ConfigureSMTPClient()
        {
            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();

            if (!String.IsNullOrEmpty(_settings.PickupDirectoryLocation))
            {
                smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.SpecifiedPickupDirectory;
                smtp.PickupDirectoryLocation = _settings.PickupDirectoryLocation;
            }
            else
            {
                smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                smtp.Port = _settings.Port;
                smtp.Host = _settings.Host;
                smtp.EnableSsl = _settings.EnableSsl;

                if (!String.IsNullOrEmpty(_settings.Username))
                {
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(_settings.Username, _settings.Password);
                }
                else
                {
                    smtp.UseDefaultCredentials = true;
                }
            }

            return smtp;
        }

    }

    public class SMTPSettings
    {

        public string DefaultSenderAccount { get; set; } = "my-application@noreply.com";

        public bool EnableSsl { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public int Port { get; set; } = 25;

        public string Host { get; set; } = "localhost";

        public string PickupDirectoryLocation { get; set; }
    }
}
