using CodeInterviewPro.Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace CodeInterviewPro.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string bodyHtml)
        {
            var host = _config["EmailSettings:SmtpHost"];
            var portString = _config["EmailSettings:SmtpPort"];
            var senderEmail = _config["EmailSettings:SenderEmail"];
            var senderName = _config["EmailSettings:SenderName"];

            // Try fetching from environment first, then fallback to appsettings
            var appPassword = Environment.GetEnvironmentVariable("SMTP_APP_PASSWORD") 
                             ?? _config["EmailSettings:AppPassword"];

            if (string.IsNullOrEmpty(appPassword))
            {
                throw new Exception("SMTP App Password is missing! Please set 'SMTP_APP_PASSWORD' in environment variables or 'EmailSettings:AppPassword' in appsettings.json.");
            }

            int port = string.IsNullOrEmpty(portString) ? 587 : int.Parse(portString);

            using var client = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(senderEmail, appPassword),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(senderEmail, senderName),
                Subject = subject,
                Body = bodyHtml,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            await client.SendMailAsync(mailMessage);
        }
    }
}
