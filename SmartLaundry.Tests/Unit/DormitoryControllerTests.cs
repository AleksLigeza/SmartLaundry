using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Moq;
using SmartLaundry.Authorization;
using SmartLaundry.Controllers;
using SmartLaundry.Data.Interfaces;
using SmartLaundry.Data.Mock;
using SmartLaundry.Models;
using SmartLaundry.Models.DormitoryViewModels;
using Xunit;

namespace SmartLaundry.Tests.Unit
{
    public class DormitoryControllerTests
    {
        private readonly DormitoryController _controller;

        public DormitoryControllerTests()
        {
            _controller = createController(null);
        }

        private DormitoryController createController(IDormitoryRepository dormitoryRepo)
        {
            if(dormitoryRepo == null)
                dormitoryRepo = new MockDormitoryRepo();

            var userRepo = new MockUserRepo();
            var roomRepo = new MockRoomRepo();
            var userManager = new Mock<MockUserManager>().Object;

            var authService = new Mock<IAuthorizationService>();
            authService
                .Setup(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<IEnumerable<IAuthorizationRequirement>>()))
                .Returns(Task.FromResult(AuthorizationResult.Success()));

            var announcementRepo = new Mock<IAnnouncementRepository>().Object;
            var localizer = new Mock<IStringLocalizer<LangResources>>().Object;

            return new DormitoryController(dormitoryRepo, userRepo, 
                roomRepo, userManager, authService.Object,
                announcementRepo, localizer);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void IndexReturnsListOfAllDormitories()
        {
            //Arrange

            //Act
            var result = _controller.Index();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var collection = Assert.IsType<List<Dormitory>>(viewResult.Model);
            Assert.Equal(3, collection.Count);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void ManageDormitoryUsersReturnsPaginatedList()
        {
            //Arrange

            //Act
            var result = await _controller.ManageDormitoryUsers(1, "", "", 1);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ManageDormitoryUsersViewModel>(viewResult.Model);
            Assert.Equal(4, model.Users.Count);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async void ManageRoomUsersReturnsPaginatedList()
        {
            //Arrange

            //Act
            var result = await _controller.ManageRoomUsers(1, "", "", 1);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ManageRoomUsersViewModel>(viewResult.Model);
            Assert.Equal(2, model.Users.Count);
        }
    }
}