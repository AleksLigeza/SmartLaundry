using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using SmartLaundry.Data;
using SmartLaundry.Models;
using SmartLaundry.Models.AccountViewModels;
using Xunit;

namespace SmartLaundry.Tests.Integration {
    public class AccountControllerTests : IClassFixture<TestFixture> {

        private readonly HttpClient _client;
        private readonly ApplicationDbContext _dbContext;
        private readonly TestFixture _fixture;

        public AccountControllerTests(TestFixture fixture) {
            _client = fixture.Client;
            _dbContext = fixture.Context;
            _fixture = fixture;
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task UserCantRegisterWithoutVerificationToken() {
            //Arrange
            var fixture = TestFixture.CreateLocalFixture();
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

        [Fact]
        [Trait("Category", "Integration")]
        public async Task UserCanRegisterWithValidVerificationToken() {
            //Arrange
            var initialResponse = await _fixture.Client.GetAsync("/account/register");
            var antiForgeryValues = await _fixture.ExtractAntiForgeryValues(initialResponse);
            var formData = new Dictionary<string, string>
            {
                {TestFixture.AntiForgeryFieldName, antiForgeryValues.fieldValue},
                {"ConfirmPassword", "abcd123"},
                {"Password", "abcd123"},
                {"Email", "abc@abc.pl"},
            };
            var postRequest = new HttpRequestMessage(HttpMethod.Post, "/account/register");
            postRequest.Headers.Add("Cookie", new CookieHeaderValue(TestFixture.AntiForgeryCookieName, antiForgeryValues.cookieValue).ToString());
            postRequest.Content = new FormUrlEncodedContent(formData);

            //Act
            var postResponse = await _fixture.Client.SendAsync(postRequest);
        
            //Assert
            Assert.True(postResponse.StatusCode == HttpStatusCode.Found);
        }
    }
}
