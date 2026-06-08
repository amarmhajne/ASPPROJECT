using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SalesManagement.Helpers;

public class AuthFilter : ActionFilterAttribute
{
    public string Role { get; set; } = string.Empty;

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var session = context.HttpContext.Session;
        var userId = session.GetInt32("UserId");
        var userRole = session.GetString("UserRole");

        if (userId == null || string.IsNullOrEmpty(userRole))
        {
            context.Result = new RedirectToActionResult("Login", "Account", null);
            return;
        }

        if (!string.IsNullOrEmpty(Role))
        {
            var allowedRoles = Role.Split(',', StringSplitOptions.TrimEntries);
            if (!allowedRoles.Contains(userRole, StringComparer.OrdinalIgnoreCase))
            {
                context.HttpContext.Session.SetString("ErrorMessage", "אין לך הרשאה לגשת לדף זה");
                context.Result = new RedirectToActionResult("Index", "Dashboard", null);
                return;
            }
        }

        base.OnActionExecuting(context);
    }
}
