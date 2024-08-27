using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;

namespace Cinema.Core.Email
{
    public class EmailService : IEmailService
    {
        private readonly EmailConfiguration _emailConfig;

        public EmailService(EmailConfiguration emailConfig)
        {
            _emailConfig = emailConfig ?? throw new ArgumentNullException(nameof(emailConfig));
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var emailMessage = CreateEmailMessage(to, subject, body);
            await SendAsync(emailMessage);
        }

        private MimeMessage CreateEmailMessage(string to, string subject, string body)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Sender Name", _emailConfig.From));
            emailMessage.To.Add(MailboxAddress.Parse(to));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = body };

            return emailMessage;
        }

        private async Task SendAsync(MimeMessage emailMessage)
        {
            using var client = new SmtpClient();
            try
            {
                await client.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.Port, true);
                await client.AuthenticateAsync(_emailConfig.Username, _emailConfig.Password);
                await client.SendAsync(emailMessage);
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                await client.DisconnectAsync(true);
                client.Dispose();
            }
        }
    }
}
