using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

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
            object controller, action;
            context.RouteData.Values.TryGetValue("controller", out controller);
            context.RouteData.Values.TryGetValue("action", out action);
            //context.HttpContext.Request.Headers
            /*
            if (context.HttpContext.Session.TryGetValue("Id", out _))
            {
                IsAuthenticated = true;
            }
            */

            if (!IsAuthenticated)
            {
                context.Result = new RedirectResult("~/Recruiters/Login?returnurl=~/" + controller.ToString() + "/" + action.ToString());
            }
            return;
        }
    }
}
