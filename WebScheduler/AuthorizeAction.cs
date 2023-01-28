using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace WebScheduler
{
    public class AuthorizeAction : IAuthorizationFilter
    {
        private readonly string role;
        public AuthorizeAction(string role)
        {
            this.role = role;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context.HttpContext.User.Identity == null || !context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.Result = new RedirectToActionResult("login", "account", null);
            }
            else
            {
                bool hasRole = context.HttpContext.User.IsInRole(role);
                if (!hasRole)
                {
                    context.Result = new ForbidResult();
                }
            }
        }
    }
}
