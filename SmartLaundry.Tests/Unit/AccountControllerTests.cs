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
        [Trait("Category", "Unit")]
        public void LoginGetReturnsLoginViewWhenNotSignedIn() {
            //Arrange
            _signInManager.Setup(manager => manager.IsSignedIn(It.IsAny<ClaimsPrincipal>()))
                .Returns(false)
                .Verifiable();

            //Act
            var result = _controller.Login();
            // Assert
            var resultView = Assert.IsType<ViewResult>(result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void LoginGetRedirectsToIndexWhenSignedIn() {
            //Arrange
            _signInManager.Setup(manager => manager.IsSignedIn(It.IsAny<ClaimsPrincipal>()))
                .Returns(true)
                .Verifiable();

            //Act
            var result = _controller.Login();
            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Home", redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void LoginPostReturnsViewModelWhenNotValidModel() {
            //Arrange
            _controller.ModelState.AddModelError("test", "test");

            //Act
            var result = await _controller.Login(new LoginViewModel());
            // Assert
            var resultView = Assert.IsType<ViewResult>(result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void LoginPostReturnsRedirectsToReturnUrlWhenSucceeded() {
            //Arrange
            _signInManager.Setup(manager => manager.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(Task.FromResult(Microsoft.AspNetCore.Identity.SignInResult.Success))
                .Verifiable();

            _controller.ModelState.Clear();

            //Act
            var result = await _controller.Login(new LoginViewModel());
            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Home", redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void LoginPostShowsErrorWhenFailed() {
            //Arrange
            _signInManager.Setup(manager => manager.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(Task.FromResult(Microsoft.AspNetCore.Identity.SignInResult.Failed))
                .Verifiable();

            //Act
            var result = await _controller.Login(new LoginViewModel());
            // Assert
            var resultView = Assert.IsType<ViewResult>(result);
            Assert.Equal("Invalid login attempt.", resultView.ViewData.ModelState[""].Errors.First().ErrorMessage);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void LoginPostShowsErrorWhenEmailIsNotConfirmed() {
            //Arrange
            _signInManager.Setup(manager => manager.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(Task.FromResult(Microsoft.AspNetCore.Identity.SignInResult.NotAllowed))
                .Verifiable();

            //Act
            var result = await _controller.Login(new LoginViewModel());
            // Assert
            var resultView = Assert.IsType<ViewResult>(result);
            Assert.Equal("Please verify your email", resultView.ViewData.ModelState[""].Errors.First().ErrorMessage);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void RegisterGetReturnsLoginViewWhenNotSignedIn() {
            //Arrange
            _signInManager.Setup(manager => manager.IsSignedIn(It.IsAny<ClaimsPrincipal>()))
                .Returns(false)
                .Verifiable();

            //Act
            var result = _controller.Register();
            // Assert
            var resultView = Assert.IsType<ViewResult>(result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void RegisterGetRedirectsToIndexWhenSignedIn() {
            //Arrange
            _signInManager.Setup(manager => manager.IsSignedIn(It.IsAny<ClaimsPrincipal>()))
                .Returns(true)
                .Verifiable();

            //Act
            var result = _controller.Register();
            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Home", redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }
    }
}