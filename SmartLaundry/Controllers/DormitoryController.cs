using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmartLaundry.Data;
using SmartLaundry.Data.Interfaces;
using SmartLaundry.Models;

namespace SmartLaundry.Controllers
{
    public class DormitoryController : Controller
    {
        private readonly IDormitoryRepository _dormitoryRepo;

        public DormitoryController(IDormitoryRepository dormitoryRepository)
        {
            _dormitoryRepo = dormitoryRepository;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(_dormitoryRepo.GetAll());
        }

        [HttpGet]
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dormitory = _dormitoryRepo.GetSingleByID(id.Value);
            if (dormitory == null)
            {
                return NotFound();
            }

            return View(dormitory);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("DormitoryID,Name,Address,ZipCode,City")] Dormitory dormitory)
        {
            if (ModelState.IsValid)
            {
                _dormitoryRepo.AddSingle(dormitory);
                return RedirectToAction(nameof(Index));
            }
            return View(dormitory);
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dormitory = _dormitoryRepo.GetSingleByID(id.Value);
            if (dormitory == null)
            {
                return NotFound();
            }
            return View(dormitory);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("DormitoryID,Name,Address,ZipCode,City")] Dormitory dormitory)
        {
            if (id != dormitory.DormitoryID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _dormitoryRepo.UpdateSingle(dormitory);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DormitoryExists(dormitory.DormitoryID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(dormitory);
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dormitory = _dormitoryRepo.GetSingleByID(id.Value);
            if (dormitory == null)
            {
                return NotFound();
            }

            return View(dormitory);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var dormitory = _dormitoryRepo.GetSingleByID(id);
            _dormitoryRepo.DeleteSingle(dormitory);
            return RedirectToAction(nameof(Index));
        }

        private bool DormitoryExists(int id)
        {
            return _dormitoryRepo.Dormitories.Any(e => e.DormitoryID == id);
        }
    }
}
