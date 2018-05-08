using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace SmartLaundry.Localization
{
    public class LanguageRouteConstraint : IRouteConstraint
    {
        public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
        {
            if(!values.ContainsKey("lang"))
            {
                return false;
            }

            var lang = values["lang"].ToString();

            return lang == "pl" || lang == "en";
        }
    }

}
