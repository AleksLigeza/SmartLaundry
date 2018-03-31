using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;


namespace SmartLaundry.Services {
    public class FakeEmailSender : IEmailSender {
        public Task SendEmailAsync(string email, string subject, string message) {
            return Task.CompletedTask;
        }
    }
}
