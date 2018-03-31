using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;


namespace SmartLaundry.Services
{
    public class EmailSender : IEmailSender {
        public EmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor) {
            Options = optionsAccessor.Value;
        }

        public AuthMessageSenderOptions Options { get; }

        public Task SendEmailAsync(string email, string subject, string message) {
            return Execute(Options.SendGridKey, subject, message, email);
        }

        public Task Execute(string apiKey, string subject, string message, string email) {
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage() {
                From = new EmailAddress("smartlaundryauto@gmail.com", "Smart Laundry"),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(email));
            return client.SendEmailAsync(msg);
        }
    }
}
