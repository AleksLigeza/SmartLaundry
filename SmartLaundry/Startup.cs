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
using SmartLaundry.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartLaundry.Controllers.Helpers;

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
            if(CurrentEnvironment.IsEnvironment("Testing") || CurrentEnvironment.IsEnvironment("SingleTest"))
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
                    options.Password.RequireDigit = true;
                    options.SignIn.RequireConfirmedEmail = false;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddTransient<IDormitoryRepository, DormitoryRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IRoomRepository, RoomRepository>();
            services.AddTransient<IReservationRepository, ReservationRepository>();
            services.AddTransient<IWashingMachineRepository, WashingMachineRepository>();
            services.AddTransient<ILaundryRepository, LaundryRepository>();
            services.AddTransient<IAnnouncementRepository, AnnouncementRepository>();

            services.AddSingleton<IAuthorizationHandler, DormitoryAuthorizationHandler>();

            services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, MyUserClaimsPrincipalFactory>();

            services.Configure<AuthMessageSenderOptions>(Configuration);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddAuthorization(options =>
            {
                options.AddPolicy("MinimumOccupant",
                    policy => policy.RequireRole("Administrator", "Manager", "Porter", "Occupant"));
                options.AddPolicy("MinimumPorter", policy => policy.RequireRole("Administrator", "Manager", "Porter"));
                options.AddPolicy("MinimumManager", policy => policy.RequireRole("Administrator", "Manager"));
                options.AddPolicy("DormitoryMembership",
                    policy => policy.Requirements.Add(new DormitoryMembershipRequirement()));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if(env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseHsts();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            RolesData.SeedRoles(app.ApplicationServices, Configuration).Wait();
            AuthHelpers.SeedRepos(app.ApplicationServices);
        }
    }
}