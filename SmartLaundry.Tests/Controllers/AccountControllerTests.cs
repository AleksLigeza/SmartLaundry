using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartLaundry.Data;
using SmartLaundry.Models;
using SmartLaundryTests.Controllers;
using Xunit;

namespace SmartLaundry.Tests.Controllers { 
    public class AccountControllerTests : IClassFixture<TestFixture<Startup>> {

        private readonly HttpClient _client;
        private readonly ApplicationDbContext _globalContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountControllerTests(TestFixture<Startup> fixture) {
            _client = fixture.Client;
            _globalContext = fixture.Context;
            _userManager = fixture.UserManager;
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async void Test1() {
            //Arrange
            var fixture = TestFixture<Startup>.CreateLocalFixture();
            var user = new ApplicationUser { UserName = "aaa", Email = "aa@aa.aa" };
            var result = await fixture.UserManager.CreateAsync(user, "aaaaa1");

            //Act
            var finding = _globalContext.Users.FirstOrDefault();

            //Assert
            Assert.True(result.Succeeded);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async void Test2() {
            //Arrange
            var fixture = TestFixture<Startup>.CreateLocalFixture();
            var user = new ApplicationUser { UserName = "aaa", Email = "aa@aa.aa" };
            var result = await fixture.UserManager.CreateAsync(user, "aaaaa1");

            //Act
            var finding = _globalContext.Users.FirstOrDefault();

            //Assert
            Assert.True(result.Succeeded);
        }
    }
}
