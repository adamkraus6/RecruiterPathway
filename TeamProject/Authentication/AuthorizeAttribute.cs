using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace TeamProject.Authentication
{
    public class AuthorizeAttribute : TypeFilterAttribute
    {
        public AuthorizeAttribute() : base(typeof(AuthorizeFilter))
        {}
    }
    public class AuthorizeFilter : IAuthorizationFilter
    {

        public AuthorizeFilter()
        {}

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var IsAuthenticated = context.HttpContext.User.Identity.IsAuthenticated;

            if (!IsAuthenticated)
            {
                context.Result = new RedirectResult("~/Recruiters/Login");
            }
            return;
        }
    }
}
