using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SmartLaundry.Data;
using SmartLaundry.Models;
using SmartLaundry.Models.AccountViewModels;
using Xunit;

namespace SmartLaundry.Tests.Integration {
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
        public async Task UserCantRegisterWithoutVerificationToken() {
            //Arrange
            var fixture = TestFixture<Startup>.CreateLocalFixture();
            var content = new {
                ConfirmPassword = "abcd123",
                Password = "abcd123",
                Email = "abc@abc.pl",
            };
            
            //Act
            var response = await fixture.Client.PostAsJsonAsync("/account/register", content);

            //Assert
            Assert.False(response.IsSuccessStatusCode);
        }
    }
}
