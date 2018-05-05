using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartLaundry.Authorization
{
    public class AuthPolicies
    {
        public static DormitoryMembershipRequirement DormitoryMembership =
            new DormitoryMembershipRequirement();
    }

    public class DormitoryMembershipRequirement : IAuthorizationRequirement { }
}
