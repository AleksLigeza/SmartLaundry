using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SmartLaundry.Data.Interfaces;
using SmartLaundry.Models;
using SmartLaundry.Models.LaundryViewModels;

namespace SmartLaundry.Controllers
{
    public class LaundryController : Controller
    {
        private readonly ILaundryRepository _laundryRepo;
        private readonly IReservationRepository _resetvationRepo;

        public LaundryController(
            ILaundryRepository laundryRepository,
            IReservationRepository reservationRepository)
        {
            _laundryRepo = laundryRepository;
            _resetvationRepo = reservationRepository;
        }

        public IActionResult Index(int id)
        {
            var laundries = _laundryRepo.GetDormitoryLaundriesWithEntities(id);

            if (laundries == null)
            {
                laundries = new List<Laundry>();
            }

            var model = new IndexViewModel()
            {
                Laundries = laundries,
                DormitoryId = id
            };

            return View(model);
        }
    }
}