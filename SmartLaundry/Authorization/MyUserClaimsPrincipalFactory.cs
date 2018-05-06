using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using SmartLaundry.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SmartLaundry.Authorization
{
    public class MyUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
    {
        public MyUserClaimsPrincipalFactory(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, roleManager, optionsAccessor)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);
            identity.AddClaim(new Claim("ManagerId", user.DormitoryManagerId.ToString() ?? ""));
            identity.AddClaim(new Claim("PorterId", user.DormitoryPorterId.ToString() ?? ""));
            identity.AddClaim(new Claim("RoomId", user.RoomId.ToString() ?? ""));
            return identity;
        }
    }
}