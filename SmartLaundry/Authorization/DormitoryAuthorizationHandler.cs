using Microsoft.AspNetCore.Authorization;
using SmartLaundry.Models;
using System.Linq;
using System.Threading.Tasks;

namespace SmartLaundry.Authorization
{
    public class DormitoryAuthorizationHandler :
        AuthorizationHandler<DormitoryMembershipRequirement, Dormitory>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            DormitoryMembershipRequirement requirement,
            Dormitory resource)
        {
            var managerId = context.User.Claims.Single(x => x.Type == "ManagerId").Value;
            var porterId = context.User.Claims.Single(x => x.Type == "PorterId").Value;
            var roomId = context.User.Claims.Single(x => x.Type == "RoomId").Value;

            if (managerId == resource.DormitoryID.ToString()
                || porterId == resource.DormitoryID.ToString()
                || resource.Rooms.Any(x => x.Id.ToString() == roomId)
                || context.User.IsInRole("Administrator"))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}