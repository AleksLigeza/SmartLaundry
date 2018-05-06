using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using SmartLaundry.Services;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace SmartLaundry.Controllers.Helpers
{
    public class Helpers
    {
        public string EmailConfirmationLink(IUrlHelper urlHelper, string userId, string code, string scheme) {
            var actionContext = new UrlActionContext {
                Action = nameof(AccountController.ConfirmEmail),
                Controller = "Account",
                Values = new { userId, code },
                Protocol = scheme
            };

            return urlHelper.Action(actionContext);
        }

        public string ResetPasswordCallbackLink(IUrlHelper urlHelper, string userId, string code, string scheme) {
            var actionContext = new UrlActionContext {
                Action = nameof(AccountController.ResetPassword),
                Controller = "Account",
                Values = new { userId, code },
                Protocol = scheme
            };

            return urlHelper.Action(actionContext);
        }

        public Task SendEmailConfirmationAsync(IEmailSender emailSender, string email, string link) {
            return emailSender.SendEmailAsync(email, "Confirm your email",
                $"Please confirm your account by clicking this link: <a href='{HtmlEncoder.Default.Encode(link)}'>link</a>");
        }
    }
}
