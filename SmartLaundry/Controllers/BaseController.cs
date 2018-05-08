using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Localization;

namespace SmartLaundry.Controllers
{
    public abstract class BaseController : Controller
    {
        private string _currentLanguage;

        public ActionResult RedirectToDefaultLanguage()
        {
            var lang = CurrentLanguage;

            return RedirectToAction("Index", new { lang = lang });
        }

        private string CurrentLanguage
        {
            get
            {
                if(string.IsNullOrEmpty(_currentLanguage))
                {
                    var feature = HttpContext.Features.Get<IRequestCultureFeature>();
                    _currentLanguage = feature.RequestCulture.Culture.TwoLetterISOLanguageName.ToLower();
                }

                return _currentLanguage;
            }
        }
    }
}
