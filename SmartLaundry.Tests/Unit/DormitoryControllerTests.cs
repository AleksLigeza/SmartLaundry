using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SmartLaundry.Controllers;
using SmartLaundry.Data.Interfaces;
using SmartLaundry.Data.Mock;
using SmartLaundry.Models;
using SmartLaundry.Models.AccountViewModels;
using SmartLaundry.Services;
using Xunit;

namespace SmartLaundry.Tests.Unit {
    public class DormitoryControllerTests {
        private readonly DormitoryController _controller;

        public DormitoryControllerTests() {
            var _dormitoryRepo = new MockDormitoryRepo();
            var _userRepo = new Mock<IUserRepository>().Object;

            _controller = new DormitoryController(_dormitoryRepo, _userRepo);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void IndexReturnsListOfAllDormitories() {
            //Arrange

            //Act
            var result = _controller.Index();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var collection = Assert.IsType<List<Dormitory>>(viewResult.Model);
            Assert.Equal(3, collection.Count);
        }
    }
}