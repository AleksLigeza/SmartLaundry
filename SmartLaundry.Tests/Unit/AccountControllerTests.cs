using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SmartLaundry.Controllers;
using SmartLaundry.Models;
using SmartLaundry.Models.AccountViewModels;
using SmartLaundry.Services;
using Xunit;

namespace SmartLaundry.Tests.Unit
{
    public class AccountControllerTests
    {
        private readonly AccountController _controller;
        private readonly Mock<MockSignInManager> _signInManager;
        private readonly Mock<MockUserManager> _userManager;
        private readonly ILogger<AccountController> _logger;

        public AccountControllerTests()
        {
            var userManager = new Mock<MockUserManager>();
            var signInManager = new Mock<MockSignInManager>();
            var logger = new Mock<ILogger<AccountController>>().Object;

            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;

            _signInManager.Setup(manager => manager.SignOutAsync())
                .Returns(Task.CompletedTask);

            _controller =
                new AccountController(userManager.Object, signInManager.Object, new FakeEmailSender(), logger);
        }

        
    }
}