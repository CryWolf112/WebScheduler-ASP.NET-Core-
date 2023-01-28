using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace WebScheduler
{
    public class AuthorizeCheckAction : IAuthorizationFilter
    {
        private readonly bool guestOnly;
        public AuthorizeCheckAction(bool guestOnly)
        {
            this.guestOnly = guestOnly;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context.HttpContext.User.Identity.IsAuthenticated && guestOnly)
            {
                if (context.HttpContext.User.IsInRole("Admin"))
                {
                    context.Result = new RedirectToActionResult("dashboard", "admin", null);
                }
                else
                {
                    context.Result = new RedirectToActionResult("dashboard", "user", null);
                }
            }
        }
    }
}
