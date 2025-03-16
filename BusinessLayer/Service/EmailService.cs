using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;
using System.Text;
using BusinessLayer.Interface;
using System.Threading.Tasks;
using ModelLayer;

namespace BusinessLayer.Service
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly SmtpSettings _smtpSettings;
        public EmailService(IConfiguration configuration, SmtpSettings smtpSettings )
        {
            _configuration = configuration;
            _smtpSettings = smtpSettings;
        }

        public void SendEmail(string toEmail, string subject, string body)
        {

            var smtpUser = Environment.GetEnvironmentVariable("SMTP_USERNAME", EnvironmentVariableTarget.User);
            var smtpPass = Environment.GetEnvironmentVariable("SMTP_PASSWORD", EnvironmentVariableTarget.User);

            if (string.IsNullOrEmpty(smtpUser) || string.IsNullOrEmpty(smtpPass))
            {
                throw new Exception("SMTP credentials are missing! Check environment variables.");
            }

            var smtpClient = new SmtpClient(_configuration["Smtp:Host"])
            {
                Port = int.Parse(_configuration["Smtp:Port"]),
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(smtpUser),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(toEmail);
            smtpClient.Send(mailMessage);
        }
    }

}
