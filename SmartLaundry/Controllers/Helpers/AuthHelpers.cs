using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartLaundry.Authorization;
using SmartLaundry.Data;
using SmartLaundry.Data.Interfaces;
using SmartLaundry.Data.Repositories;
using SmartLaundry.Models;

namespace SmartLaundry.Controllers.Helpers
{
    public class AuthHelpers
    {
        private readonly IDormitoryRepository _dormitoryRepo;
        private readonly IAuthorizationService _authorizationService;

        public AuthHelpers(IAuthorizationService authorizationService, IDormitoryRepository dormitoryRepo = null)
        {
            _dormitoryRepo = dormitoryRepo;
            _authorizationService = authorizationService;
        }

        public  async Task<bool> CheckDormitoryMembership(ClaimsPrincipal user, Dormitory dormitory)
        {
            var authorizationResult =
                await _authorizationService.AuthorizeAsync(user, dormitory, AuthPolicies.DormitoryMembership);
            return authorizationResult.Succeeded;
        }

        public  async Task<bool> CheckDormitoryMembership(ClaimsPrincipal user, int dormitoryId)
        {
            var dormitory = _dormitoryRepo.GetSingleById(dormitoryId);
            return await CheckDormitoryMembership(user, dormitory);
        }
    }
}
