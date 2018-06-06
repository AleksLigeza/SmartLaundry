using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Moq;
using SmartLaundry.Controllers;
using SmartLaundry.Data.Interfaces;
using SmartLaundry.Data.Mock;
using SmartLaundry.Models;
using SmartLaundry.Models.AccountViewModels;
using SmartLaundry.Models.DormitoryViewModels;
using SmartLaundry.Models.HomeViewModels;
using SmartLaundry.Services;
using Xunit;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace SmartLaundry.Tests.Unit
{
    public class HomeControllerTests
    {
        private readonly HomeController _controller;
        private readonly Mock<MockSignInManager> _signInManager;
        private readonly Mock<MockUserManager> _userManager;
        private readonly ILogger<AccountController> _logger;

        public HomeControllerTests()
        {
            var userManager = new Mock<MockUserManager>();
            var signInManager = new Mock<MockSignInManager>();
            var logger = new Mock<ILogger<AccountController>>().Object;
            var laundryRepo = new Mock<ILaundryRepository>().Object;
            var userRepo = new Mock<MockUserRepo>().Object;
            var reservationRepo = new Mock<IReservationRepository>().Object;
            var announcementsRepo = new Mock<IAnnouncementRepository>().Object;
            var localizer = new Mock<IStringLocalizer<LangResources>>();

            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;

            _signInManager.Setup(manager => manager.SignOutAsync())
                .Returns(Task.CompletedTask);


            _controller =
                new HomeController(userManager.Object, signInManager.Object, 
                    new FakeEmailSender(), logger, 
                    laundryRepo, userRepo,
                    reservationRepo, announcementsRepo,
                    localizer.Object);

        }

        [Fact]
        [Trait("Category", "Unit")]
        public void IndexReturnsModelWithLoginAndRegisterViewModels()
        {
            //Arrange
            _signInManager.Setup(manager => manager.IsSignedIn(It.IsAny<ClaimsPrincipal>()))
                .Returns(false);

            //Act
            var result = _controller.Index();
            // Assert
            var resultView = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<HomeIndexViewModel>(resultView.Model);
            Assert.NotNull(model.LoginViewModel);
            Assert.NotNull(model.RegisterViewModel);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void LoginPostRedirectsToIndexWhenSignedIn()
        {
            //Arrange
            var model = new LoginViewModel()
            {
                Email = "",
                Password = "",
                RememberMe = true
            };
            _signInManager.Setup(manager => manager.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
                .Returns(Task.FromResult(SignInResult.Success));

            //Act
            var result = await _controller.Login(model);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Home", redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void LoginPostReturnsViewModelWhenNotValidModel()
        {
            //Arrange
            _controller.ModelState.AddModelError("test", "test");

            //Act
            var result = await _controller.Login(new LoginViewModel());
            // Assert
            var resultView = Assert.IsType<ViewResult>(result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void LoginPostReturnsRedirectsToReturnUrlWhenSucceeded()
        {
            //Arrange
            _signInManager.Setup(manager =>
                    manager.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(),
                        It.IsAny<bool>()))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

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
        public async void RegisterPostReturnsViewModelWhenModeStatelIsNotValid()
        {
            //Arrange
            _controller.ModelState.AddModelError("test", "test");

            //Act
            var result = await _controller.Register(new RegisterViewModel());

            // Assert
            var resultView = Assert.IsType<ViewResult>(result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void RegisterPostShowsErrorWhenFailed()
        {
            //Arrange
            _userManager.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError() { Description = "test" }));

            //Act
            var result = await _controller.Register(new RegisterViewModel());
            // Assert
            var resultView = Assert.IsType<ViewResult>(result);
            Assert.NotEmpty(resultView.ViewData.ModelState[""].Errors);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void RegisterPostRedirectsToIndexWhenSucceeded()
        {
            //Arrange
            _userManager.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            _userManager.Setup(m => m.GenerateEmailConfirmationTokenAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync("test");

            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(x => x.Action(It.IsAny<Microsoft.AspNetCore.Mvc.Routing.UrlActionContext>()))
                .Returns("test");

            _controller.Url = urlHelper.Object;
            _controller.ModelState.Clear();

            TestHelpers.SetFakeHttpRequestSchemeTo(_controller, It.IsAny<string>());

            //Act
            var result = await _controller.Register(new RegisterViewModel() { Email = "" });

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Home", redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);
        }
    }
}
