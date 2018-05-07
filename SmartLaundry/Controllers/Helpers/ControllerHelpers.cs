using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using SmartLaundry.Services;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SmartLaundry.Models;

namespace SmartLaundry.Controllers.Helpers
{
    public static class ControllerHelpers
    {
        public static string EmailConfirmationLink(IUrlHelper urlHelper, string userId, string code, string scheme)
        {
            var actionContext = new UrlActionContext
            {
                Action = nameof(AccountController.ConfirmEmail),
                Controller = "Account",
                Values = new {userId, code},
                Protocol = scheme
            };

            return urlHelper.Action(actionContext);
        }

        public static string ResetPasswordCallbackLink(IUrlHelper urlHelper, string userId, string code, string scheme)
        {
            var actionContext = new UrlActionContext
            {
                Action = nameof(AccountController.ResetPassword),
                Controller = "Account",
                Values = new {userId, code},
                Protocol = scheme
            };

            return urlHelper.Action(actionContext);
        }

        public static Task SendEmailConfirmationAsync(IEmailSender emailSender, string email, string link)
        {
            return emailSender.SendEmailAsync(email, "Confirm your email",
                $"Please confirm your account by clicking this link: <a href='{HtmlEncoder.Default.Encode(link)}'>link</a>");
        }

        public static  IActionResult ShowErrorPage(ControllerBase controller, string message = "", string title = "")
        {
            return controller.RedirectToAction(nameof(HomeController.Error), "Home", new { message = message, title = title });
        }

        public static IActionResult Show404ErrorPage(ControllerBase controller)
        {
            return ShowErrorPage(controller, "The resource can not be found", "404 - Page not found");
        }

        public static IActionResult ShowAccessDeniedErrorPage(ControllerBase controller)
        {
            return controller.RedirectToAction(nameof(AccountController.AccessDenied), "Account");
        }
    }
}