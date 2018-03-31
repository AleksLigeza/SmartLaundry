using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SmartLaundry.Controllers;
using SmartLaundry.Models;
using SmartLaundry.Models.AccountViewModels;
using SmartLaundry.Services;
using Xunit;

namespace SmartLaundry.Tests.Unit {
    public class AccountControllerTests {

        private readonly AccountController _controller;
        private readonly Mock<MockSignInManager> _signInManager;
        private readonly Mock<MockUserManager> _userManager;
        private readonly Mock<FakeEmailSender> _emailSender;
        private readonly ILogger<AccountController> _logger;

        public AccountControllerTests() {
            var userManager = new Mock<MockUserManager>();
            var signInManager = new Mock<MockSignInManager>();
            var emailSender = new Mock<FakeEmailSender>();
            var logger = new Mock<ILogger<AccountController>>().Object;

            _signInManager = signInManager;
            _userManager = userManager;
            _emailSender = emailSender;
            _logger = logger;

            _signInManager.Setup(manager => manager.SignOutAsync())
                .Returns(Task.CompletedTask)
                .Verifiable();

            _controller = new AccountController(userManager.Object, signInManager.Object, emailSender.Object, logger);
        }

        [Fact]
        public async void LoginReturnsLoginViewWhenNotSignedIn() {
            //Arrange
            _signInManager.Setup(manager => manager.IsSignedIn(It.IsAny<ClaimsPrincipal>()))
                .Returns(false)
                .Verifiable();

            //Act
            var result = await _controller.Login();
            // Assert
            var resultView = Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async void LoginRedirectsToIndexWhenSignedIn() {
            //Arrange
            _signInManager.Setup(manager => manager.IsSignedIn(It.IsAny<ClaimsPrincipal>()))
                .Returns(true)
                .Verifiable();

            //Act
            var result = await _controller.Login();
            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Home", redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }
    }
}