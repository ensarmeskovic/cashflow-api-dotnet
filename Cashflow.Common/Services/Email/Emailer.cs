using System.Linq;
using System.Threading.Tasks;
using Cashflow.Core;
using Cashflow.Core.Configurations;
using MailKit.Net.Smtp;
using MimeKit;

namespace Cashflow.Common.Services.Email
{
    public class Emailer : IEmailer
    {
        private readonly EmailConfiguration _emailConfiguration;

        public Emailer(EmailConfiguration emailConfiguration)
        {
            _emailConfiguration = emailConfiguration;
        }

        public bool Send(string subject, string message, string senderEmail, string receiverEmail)
        {
            using (SmtpClient client = new MailKit.Net.Smtp.SmtpClient())
            {
                client.Connect(_emailConfiguration.Host, _emailConfiguration.Port, MailKit.Security.SecureSocketOptions.SslOnConnect);
                client.Authenticate(_emailConfiguration.Username, _emailConfiguration.Password);

                MimeMessage mailMessage = new MimeMessage
                {
                    Body = new TextPart(MimeKit.Text.TextFormat.Html)
                    {
                        Text = message
                    },
                    From =
                    {
                        new MailboxAddress(_emailConfiguration.DisplayName, senderEmail)
                    },
                    To =
                    {
                        new MailboxAddress(receiverEmail)
                    },
                    Subject = subject
                };

                client.Send(mailMessage);
                client.Disconnect(true);
            }

            return true;
        }
        public async Task<bool> SendAsync(string subject, string message, string senderEmail, string receiverEmail)
        {
            using (SmtpClient client = new MailKit.Net.Smtp.SmtpClient())
            {
                await client.ConnectAsync(_emailConfiguration.Host, _emailConfiguration.Port);
                await client.AuthenticateAsync(_emailConfiguration.Username, _emailConfiguration.Password);

                MimeMessage mailMessage = new MimeMessage
                {
                    Body = new TextPart(MimeKit.Text.TextFormat.Html)
                    {
                        Text = message
                    },
                    From =
                    {
                        new MailboxAddress(_emailConfiguration.DisplayName, senderEmail)
                    },
                    To =
                    {
                        new MailboxAddress(receiverEmail)
                    },
                    Subject = subject
                };

                await client.SendAsync(mailMessage);
                await client.DisconnectAsync(true);
            }

            return true;
        }

        public bool Send(string subject, string message, SenderTypes senderType, string receiverEmail)
        {
            EmailSender emailConfigurationSender = _emailConfiguration.Senders.FirstOrDefault(x => x.Type.Equals(senderType));
            if (emailConfigurationSender == null)
                return false;

            using (SmtpClient client = new MailKit.Net.Smtp.SmtpClient())
            {
                client.Connect(_emailConfiguration.Host, _emailConfiguration.Port);
                client.Authenticate(emailConfigurationSender.Username, emailConfigurationSender.Password);

                MimeMessage mailMessage = new MimeMessage
                {
                    Body = new TextPart(MimeKit.Text.TextFormat.Html)
                    {
                        Text = message
                    },
                    From =
                    {
                        new MailboxAddress(_emailConfiguration.DisplayName, emailConfigurationSender.Username)
                    },
                    To =
                    {
                        new MailboxAddress(receiverEmail)
                    },
                    Subject = subject
                };

                client.Send(mailMessage);
                client.Disconnect(true);
            }

            return true;
        }
        public async Task<bool> SendAsync(string subject, string message, SenderTypes senderType, string receiverEmail)
        {
            EmailSender emailConfigurationSender = _emailConfiguration.Senders.FirstOrDefault(x => x.Type.Equals(senderType));
            if (emailConfigurationSender == null)
                return false;

            using (SmtpClient client = new MailKit.Net.Smtp.SmtpClient())
            {
                await client.ConnectAsync(_emailConfiguration.Host, _emailConfiguration.Port);
                await client.AuthenticateAsync(emailConfigurationSender.Username, emailConfigurationSender.Password);

                MimeMessage mailMessage = new MimeMessage
                {
                    Body = new TextPart(MimeKit.Text.TextFormat.Html)
                    {
                        Text = message
                    },
                    From =
                    {
                        new MailboxAddress(_emailConfiguration.DisplayName, emailConfigurationSender.Username)
                    },
                    To =
                    {
                        new MailboxAddress(receiverEmail)
                    },
                    Subject = subject
                };

                await client.SendAsync(mailMessage);
                await client.DisconnectAsync(true);
            }

            return true;
        }
    }
}
