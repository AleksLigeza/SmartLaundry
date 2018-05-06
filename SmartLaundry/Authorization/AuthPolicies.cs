using Microsoft.AspNetCore.Authorization;

namespace SmartLaundry.Authorization
{
    public class AuthPolicies
    {
        public static DormitoryMembershipRequirement DormitoryMembership =
            new DormitoryMembershipRequirement();
    }

    public class DormitoryMembershipRequirement : IAuthorizationRequirement
    {
    }
}