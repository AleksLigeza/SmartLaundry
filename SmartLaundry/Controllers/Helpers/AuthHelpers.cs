using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using SmartLaundry.Authorization;
using SmartLaundry.Data.Interfaces;
using SmartLaundry.Data.Repositories;
using SmartLaundry.Models;

namespace SmartLaundry.Controllers.Helpers
{
    public class AuthHelpers
    {
        public static async Task<bool> CheckDormitoryMembership(ClaimsPrincipal user, AuthRepositories repos, Dormitory dormitory)
        {
            var authorizationResult =
                await repos.AuthorizationService.AuthorizeAsync(user, dormitory, AuthPolicies.DormitoryMembership);
            return authorizationResult.Succeeded;
        }

        public static async Task<bool> CheckDormitoryMembership(ClaimsPrincipal user, AuthRepositories repos, int dormitoryId)
        {
            var dormitory = repos.DormitoryRepo.GetSingleById(dormitoryId);
            return await CheckDormitoryMembership(user, repos, dormitory);
        }

        public static async Task<bool> CheckDormitoryMembership(ClaimsPrincipal user, AuthRepositories repos, WashingMachine machine)
        {
            var laundry = repos.LaundryRepo.GetLaundryById(machine.LaundryId);

            if (laundry == null)
                return await CheckDormitoryMembership(user, repos, null as Dormitory);

            var dormitoryId = laundry.DormitoryId;
            return await CheckDormitoryMembership(user, repos, dormitoryId);
        }

        public static async Task<bool> CheckDormitoryMembership(ClaimsPrincipal user, AuthRepositories repos, Reservation reservation)
        {
            var machine = repos.WashingMachineRepo.GetWashingMachineById(reservation.WashingMachineId);

            if(machine == null)
                return await CheckDormitoryMembership(user, repos, null as Dormitory);

            return await CheckDormitoryMembership(user, repos, machine);

        }

        public class AuthRepositories
        {
            public IDormitoryRepository DormitoryRepo;
            public ILaundryRepository LaundryRepo;
            public IWashingMachineRepository WashingMachineRepo;
            public IAuthorizationService AuthorizationService;
        }
    }
}
