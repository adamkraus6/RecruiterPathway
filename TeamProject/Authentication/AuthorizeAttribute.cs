using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

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
            /*
            if (context.HttpContext.Session.TryGetValue("Id", out _))
            {
                IsAuthenticated = true;
            }
            */

            if (!IsAuthenticated)
            {
                context.Result = new RedirectResult("~/Recruiters/Login");
            }
            return;
        }
    }
}
