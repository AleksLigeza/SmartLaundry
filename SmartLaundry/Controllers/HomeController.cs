using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SmartLaundry.Controllers.Helpers;
using SmartLaundry.Data.Interfaces;
using SmartLaundry.Models;
using SmartLaundry.Models.AccountViewModels;
using SmartLaundry.Models.HomeViewModels;
using SmartLaundry.Services;

namespace SmartLaundry.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ILaundryRepository _laundryRepo;
        private readonly IUserRepository _userRepo;
        private readonly IReservationRepository _reservationRepo;
        private readonly IAnnouncementRepository _announcementRepo;

        public HomeController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender,
            ILogger<AccountController> logger,
            ILaundryRepository laundryRepository,
            IUserRepository userRepository,
            IReservationRepository reservationRepository,
            IAnnouncementRepository announcementRepo)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _laundryRepo = laundryRepository;
            _userRepo = userRepository;
            _reservationRepo = reservationRepository;
            _announcementRepo = announcementRepo;
        }

        [HttpGet]
        public IActionResult Index(string returnUrl = null)
        {
            HomeIndexViewModel model;

            if(_signInManager.IsSignedIn(User))
            {
                var userId = _userManager.GetUserId(User);
                var user = _userRepo.GetUserById(userId);

                int? dormitoryId = user.Room?.DormitoryId ?? user.DormitoryManagerId ?? user.DormitoryPorterId;

                if(dormitoryId != null)
                {
                    model = new HomeIndexViewModel()
                    {
                        DayViewModel = ControllerHelpers.CreateDayViewModel(dormitoryId.Value, DateTime.Today.Date,
                            userId, _laundryRepo, _reservationRepo, _userRepo),
                        Announcements = _announcementRepo.GetDormitoryAnnouncements(dormitoryId.Value),
                        Dormitory = user.Room?.Dormitory ?? user.DormitoryManager ?? user.DormitoryPorter
                    };
                    return View(model);
                }

                RedirectToAction(nameof(DormitoryController.Index), "Dormitory");
            }

            ViewData["ReturnUrl"] = returnUrl;

            model = new HomeIndexViewModel()
            {
                LoginViewModel = new LoginViewModel(),
                RegisterViewModel = new RegisterViewModel()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["RequestForm"] = "login";
            ViewData["ReturnUrl"] = returnUrl;
            if(ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe,
                    lockoutOnFailure: false);
                if(result.Succeeded)
                {
                    return RedirectToLocal(returnUrl);
                }
                else if(result.IsNotAllowed)
                {
                    ModelState.AddModelError(string.Empty, "Please verify your email");
                    return ReturnIndexView(model);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return ReturnIndexView(model);
                }
            }

            return ReturnIndexView(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["RequestForm"] = "register";
            ViewData["ReturnUrl"] = returnUrl;
            if(ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Firstname = model.Firstname,
                    Lastname = model.Lastname
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if(result.Succeeded)
                {
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = ControllerHelpers.EmailConfirmationLink(Url, user.Id, code, Request.Scheme);
                    await ControllerHelpers.SendEmailConfirmationAsync(_emailSender, model.Email, callbackUrl);

                    await _signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: true, lockoutOnFailure: false);

                    return RedirectToLocal(returnUrl);
                }

                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return ReturnIndexView(model);

        }

        private IActionResult ReturnIndexView(RegisterViewModel model)
        {
            return View(nameof(Index),
                new HomeIndexViewModel() { RegisterViewModel = model, LoginViewModel = new LoginViewModel() });
        }

        private IActionResult ReturnIndexView(LoginViewModel model)
        {
            return View(nameof(Index),
                new HomeIndexViewModel() { LoginViewModel = model, RegisterViewModel = new RegisterViewModel() });
        }

        [HttpGet]
        public IActionResult Error(string message, string title)
        {
            var model = new ErrorViewModel
            {
                Message = message,
                Title = title
            };
            return View(model);
        }


        public IActionResult ChangeCulture(string lang, string returnUrl)
        {
            if (returnUrl.Contains("pl-PL") || returnUrl.Contains("en-US"))
            {
                returnUrl = returnUrl.Substring(6);
            }

            return Redirect("/" + lang + returnUrl);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach(var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if(returnUrl != null && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }
    }
}