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
        public static void SeedRepos(IServiceProvider serviceProvider)
        {
            using(var serviceScope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                AuthRepositories.DormitoryRepo = serviceScope.ServiceProvider.GetRequiredService<IDormitoryRepository>();
                AuthRepositories.LaundryRepo = serviceScope.ServiceProvider.GetRequiredService<ILaundryRepository>();
                AuthRepositories.WashingMachineRepo = serviceScope.ServiceProvider.GetRequiredService<IWashingMachineRepository>();
                AuthRepositories.AuthorizationService = serviceScope.ServiceProvider.GetRequiredService<IAuthorizationService>();             
            }
        }

        public static async Task<bool> CheckDormitoryMembership(ClaimsPrincipal user, Dormitory dormitory)
        {
            var authorizationResult =
                await AuthRepositories.AuthorizationService.AuthorizeAsync(user, dormitory, AuthPolicies.DormitoryMembership);
            return authorizationResult.Succeeded;
        }

        public static async Task<bool> CheckDormitoryMembership(ClaimsPrincipal user, int dormitoryId)
        {
            var dormitory = AuthRepositories.DormitoryRepo.GetSingleById(dormitoryId);
            return await CheckDormitoryMembership(user, dormitory);
        }

        public static async Task<bool> CheckDormitoryMembership(ClaimsPrincipal user, WashingMachine machine)
        {
            var laundry = AuthRepositories.LaundryRepo.GetLaundryById(machine.LaundryId);

            if(laundry == null)
                return await CheckDormitoryMembership(user, null as Dormitory);

            var dormitoryId = laundry.DormitoryId;
            return await CheckDormitoryMembership(user, dormitoryId);
        }

        public static async Task<bool> CheckDormitoryMembership(ClaimsPrincipal user, Reservation reservation)
        {
            var machine = AuthRepositories.WashingMachineRepo.GetWashingMachineById(reservation.WashingMachineId);

            if(machine == null)
                return await CheckDormitoryMembership(user, null as Dormitory);

            return await CheckDormitoryMembership(user, machine);
        }

        internal static class AuthRepositories
        {
            public static IDormitoryRepository DormitoryRepo;
            public static ILaundryRepository LaundryRepo;
            public static IWashingMachineRepository WashingMachineRepo;
            public static IAuthorizationService AuthorizationService;
        }
    }
}
