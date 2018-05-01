using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartLaundry.Data;
using SmartLaundry.Models;
using SmartLaundry.Services;
using SmartLaundry.Data.Interfaces;
using SmartLaundry.Data.Repositories;
using System.Reflection;

namespace SmartLaundry
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment currentEnvironment)
        {
            Configuration = configuration;
            CurrentEnvironment = currentEnvironment;
        }

        public IConfiguration Configuration { get; }
        public IHostingEnvironment CurrentEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            if (CurrentEnvironment.IsEnvironment("Testing") || CurrentEnvironment.IsEnvironment("SingleTest"))
            {
                services.AddDbContext<ApplicationDbContext>(optionsBuilder =>
                    optionsBuilder
                        .UseInMemoryDatabase(CurrentEnvironment.EnvironmentName)
                    );

                services.AddTransient<IEmailSender, FakeEmailSender>();
            }
            else
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                    options
                        .UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

                services.AddTransient<IEmailSender, EmailSender>();
            }

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = false;
                options.SignIn.RequireConfirmedEmail = true;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Add application services.
            services.AddTransient<IDormitoryRepository, DormitoryRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IRoomRepository, RoomRepository>();
            services.AddTransient<IReservationRepository, ReservationRepository>();
            services.AddTransient<ILaundryRepository, LaundryRepository>();

            services.Configure<AuthMessageSenderOptions>(Configuration);

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
