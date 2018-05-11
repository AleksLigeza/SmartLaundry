using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SmartLaundry.Data;
using SmartLaundry.Models;
using Xunit;

namespace SmartLaundry.Tests.Integration
{
    public class HomeControllerTests : IClassFixture<TestFixture>
    {
        private readonly HttpClient _client;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly TestFixture _fixture;

        public HomeControllerTests(TestFixture fixture)
        {
            _client = fixture.Client;
            _dbContext = fixture.Context;
            _fixture = fixture;
            _client = fixture.Client;
            _userManager = fixture.UserManager;
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task EmptyPathReturnsIndexPage()
        {
            // Arrange
            // ---

            // Arrange & Act
            var response = await _client.GetAsync("/");

            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Contains("<title>Home Page - SmartLaundry</title>", responseString);
        }


        [Fact]
        [Trait("Category", "Integration")]
        public async Task UserCantRegisterWithoutVerificationToken()
        {
            //Arrange
            var fixture = TestFixture.CreateLocalFixture();
            var content = new
            {
                ConfirmPassword = "abcd123",
                Password = "abcd123",
                Email = "abc@abc.pl",
                Firstname = "Jan",
                Lastname = "Kowalski"
            };

            //Act
            var response = await fixture.Client.PostAsJsonAsync("/en-US/Home/Register", content);

            //Assert
            Assert.False(response.IsSuccessStatusCode);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task UserCanRegisterWithValidVerificationToken()
        {
            //Arrange
            var fixture = TestFixture.CreateLocalFixture();
            var initialResponse = await fixture.Client.GetAsync("/en-US/");
            var antiForgeryValues = await fixture.ExtractAntiForgeryValues(initialResponse);
            var formData = new Dictionary<string, string>
            {
                {TestFixture.AntiForgeryFieldName, antiForgeryValues.fieldValue},
                {"ConfirmPassword", "abcd123"},
                {"Password", "abcd123"},
                {"Email", "abc@abc.pl"},
                {"Firstname", "Jan"},
                {"Lastname", "Kowalski"}
            };
            var postRequest = new HttpRequestMessage(HttpMethod.Post, "/en-US/Home/Register");
            postRequest.Headers.Add("Cookie",
                new Microsoft.Net.Http.Headers.CookieHeaderValue(TestFixture.AntiForgeryCookieName, antiForgeryValues.cookieValue).ToString());
            postRequest.Content = new FormUrlEncodedContent(formData);

            //Act
            var postResponse = await fixture.Client.SendAsync(postRequest);

            //Assert
            Assert.Equal(HttpStatusCode.Found, postResponse.StatusCode);
            Assert.Equal("/en-US", postResponse.Headers.Location.ToString());
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task UserCantLoginWithoutValidData()
        {
            //Arrange
            var fixture = TestFixture.CreateLocalFixture();

            var user = new ApplicationUser { UserName = "abc@abc.pl", Email = "abc@abc.pl" };
            var result = await fixture.UserManager.CreateAsync(user, "abcd123");

            var initialResponse = await _fixture.Client.GetAsync("/");
            var antiForgeryValues = await _fixture.ExtractAntiForgeryValues(initialResponse);
            var formData = new Dictionary<string, string>
            {
                {TestFixture.AntiForgeryFieldName, antiForgeryValues.fieldValue},
                {"Password", "abcd12345"},
                {"Email", "abc@abc.pl"},
                {"RememberMe", "true"}
            };
            var postRequest = new HttpRequestMessage(HttpMethod.Post, "/en-US/Home/Login");
            postRequest.Headers.Add("Cookie",
                new Microsoft.Net.Http.Headers.CookieHeaderValue(TestFixture.AntiForgeryCookieName, antiForgeryValues.cookieValue).ToString());
            postRequest.Content = new FormUrlEncodedContent(formData);

            //Act
            var postResponse = await fixture.Client.SendAsync(postRequest);

            //Assert
            string content = await postResponse.Content.ReadAsStringAsync();
            Assert.Contains("Invalid login attempt", content);
        }


        [Fact]
        [Trait("Category", "Integration")]
        public async Task UserCanLoginWithValidData()
        {
            //Arrange
            var fixture = TestFixture.CreateLocalFixture();

            var user = new ApplicationUser { UserName = "abc@abc.pl", Email = "abc@abc.pl" };
            var result = await fixture.UserManager.CreateAsync(user, "abcd123");

            var userToConfirm = fixture.Context.Users.Where(u => u.UserName == "abc@abc.pl").SingleOrDefault();
            userToConfirm.EmailConfirmed = true;
            fixture.Context.Users.Update(userToConfirm);
            fixture.Context.SaveChanges();

            var initialResponse = await _fixture.Client.GetAsync("/en-US/");
            var antiForgeryValues = await _fixture.ExtractAntiForgeryValues(initialResponse);
            var formData = new Dictionary<string, string>
            {
                {TestFixture.AntiForgeryFieldName, antiForgeryValues.fieldValue},
                {"Password", "abcd123"},
                {"Email", "abc@abc.pl"},
                {"RememberMe", "true"}
            };
            var postRequest = new HttpRequestMessage(HttpMethod.Post, "/en-US/Home/Login");
            postRequest.Headers.Add("Cookie",
                new Microsoft.Net.Http.Headers.CookieHeaderValue(TestFixture.AntiForgeryCookieName, antiForgeryValues.cookieValue).ToString());
            postRequest.Content = new FormUrlEncodedContent(formData);

            //Act
            var postResponse = await fixture.Client.SendAsync(postRequest);

            //Assert
            Assert.Equal(HttpStatusCode.Found, postResponse.StatusCode);
            Assert.Equal("/en-US", postResponse.Headers.Location.ToString());
        }
    }
}