//Based on:     ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- -----
//https://github.com/aspnet/Docs/blob/master/aspnetcore/mvc/controllers/testing/sample/TestingControllersSample/tests/TestingControllersSample.Tests/IntegrationTests/TestFixture.cs
//  ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- ----- -----

using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using SmartLaundry.Data;
using SmartLaundry.Models;

namespace SmartLaundry.Tests.Integration {
    public class TestFixture<TStartup> : IDisposable {

        private readonly TestServer _server;

        public HttpClient Client { get; }
        public ApplicationDbContext Context { get; }
        public UserManager<ApplicationUser> UserManager { get; }

        public static TestFixture<TStartup> CreateLocalFixture() {
            return new TestFixture<TStartup>(Path.Combine("SmartLaundry"), "SingleTest");
        }

        public TestFixture() 
            : this(Path.Combine("SmartLaundry"), "Testing") {
        }

        protected TestFixture(string relativeTargetProjectParentDir, string envirnoment) {
            var startupAssembly = typeof(TStartup).GetTypeInfo().Assembly;
            var contentRoot = GetProjectPath(relativeTargetProjectParentDir, startupAssembly);

            var builder = new WebHostBuilder()
                .UseContentRoot(contentRoot)
                .ConfigureServices(InitializeServices)
                .UseEnvironment(envirnoment)
                .UseStartup(typeof(TStartup));

            _server = new TestServer(builder);

            Client = _server.CreateClient();
            Client.BaseAddress = new Uri("http://localhost");

            UserManager = _server.Host.Services.GetService(typeof(UserManager<ApplicationUser>)) as UserManager<ApplicationUser>;
            Context = _server.Host.Services.GetService(typeof(ApplicationDbContext)) as ApplicationDbContext;
        }

        public void Dispose() {
            Client.Dispose();
            _server.Dispose();
        }

        protected virtual void InitializeServices(IServiceCollection services) {
            var startupAssembly = typeof(TStartup).GetTypeInfo().Assembly;

            var manager = new ApplicationPartManager();
            manager.ApplicationParts.Add(new AssemblyPart(startupAssembly));
            manager.FeatureProviders.Add(new ControllerFeatureProvider());
            manager.FeatureProviders.Add(new ViewComponentFeatureProvider());

            services.AddSingleton(manager);
        }


        private static string GetProjectPath(string projectRelativePath, Assembly startupAssembly) {
            var projectName = startupAssembly.GetName().Name;

            var applicationBasePath = System.AppContext.BaseDirectory;

            var directoryInfo = new DirectoryInfo(applicationBasePath);
            do {
                directoryInfo = directoryInfo.Parent;

                var projectDirectoryInfo = new DirectoryInfo(Path.Combine(directoryInfo.FullName, projectRelativePath));
                if (projectDirectoryInfo.Exists) {
                    var projectFileInfo = new FileInfo(Path.Combine(projectDirectoryInfo.FullName, projectName, $"{projectName}.csproj"));
                    if (projectFileInfo.Exists) {
                        return Path.Combine(projectDirectoryInfo.FullName, projectName);
                    }
                }
            }
            while (directoryInfo.Parent != null);

            throw new Exception($"Project root could not be located using the application root {applicationBasePath}.");
        }
    }
}
