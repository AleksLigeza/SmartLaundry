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
        private readonly ApplicationDbContext _globalContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeControllerTests(TestFixture fixture)
        {
            _client = fixture.Client;
            _globalContext = fixture.Context;
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
    }
}