using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HireSphere.Filters
{
    /// <summary>
    /// Custom authorization attribute to protect routes based on user role
    /// </summary>
    public class AuthorizeRoleAttribute : ActionFilterAttribute
    {
        private readonly string _requiredRole;

        public AuthorizeRoleAttribute(string role)
        {
            _requiredRole = role;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var userRole = context.HttpContext.Session.GetString("RoleId");
            var userId = context.HttpContext.Session.GetString("UserId");

            // Check if user is logged in
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userRole))
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }

            // Check if user has the required role
            if (userRole != _requiredRole)
            {
                // Redirect to appropriate dashboard or show unauthorized
                if (userRole == "1")
                {
                    context.Result = new RedirectToActionResult("Dashboard", "Admin", null);
                }
                else if (userRole == "2")
                {
                    context.Result = new RedirectToActionResult("Dashboard", "Client", null);
                }
                else
                {
                    context.Result = new RedirectToActionResult("Login", "Account", null);
                }
                return;
            }

            base.OnActionExecuting(context);
        }
    }

    /// <summary>
    /// Attribute to ensure user is logged in (any role)
    /// </summary>
    public class RequireLoginAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var userId = context.HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
