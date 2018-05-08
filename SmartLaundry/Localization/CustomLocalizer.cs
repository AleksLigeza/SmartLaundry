using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Localization;

namespace SmartLaundry.Localization
{
    public class CustomLocalizer : StringLocalizer<StringResource>
    {
        private readonly IStringLocalizer _internalLocalizer;

        public CustomLocalizer(IStringLocalizerFactory factory) : base(factory)
        {

        }
        public CustomLocalizer(IStringLocalizerFactory factory, IHttpContextAccessor httpContextAccessor) : base(factory)
        {
            CurrentLanguage = httpContextAccessor.HttpContext.GetRouteValue("lang") as string;
            if(string.IsNullOrEmpty(CurrentLanguage))
            {
                CurrentLanguage = "pl";
            }

            _internalLocalizer = WithCulture(new CultureInfo(CurrentLanguage));
        }

        public override LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                return _internalLocalizer[name, arguments];
            }
        }

        public override LocalizedString this[string name]
        {
            get
            {
                return _internalLocalizer[name];
            }
        }

        public string CurrentLanguage { get; set; }
    }
}
