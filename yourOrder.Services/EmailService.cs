using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using yourOrder.Core.Services;

namespace yourOrder.Services
{
    public class EmailService  : IEmailService
    {
        private readonly IConfiguration _config;
        public EmailService(IConfiguration config) =>  _config = config;
        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var smtpSettings = _config.GetSection("SmtpSettings");
            var email = smtpSettings["Email"];
            var password = smtpSettings["Password"];
            var host = smtpSettings["Host"];
            var port = int.Parse(smtpSettings["Port"]);

            using (var client = new SmtpClient(host, port))
            {
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(email, password);
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(email, "User Management System"),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(toEmail);
                await client.SendMailAsync(mailMessage);
            }
        }
    }
}
