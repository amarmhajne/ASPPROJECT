using Microsoft.AspNetCore.Mvc;
using SalesManagement.Helpers;

namespace SalesManagement.Controllers;

[AuthFilter]
public class DashboardController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        ViewBag.UserRole = HttpContext.Session.GetString("UserRole");
        ViewBag.FullName = HttpContext.Session.GetString("FullName");

        var errorMessage = HttpContext.Session.GetString("ErrorMessage");
        if (!string.IsNullOrEmpty(errorMessage))
        {
            ViewBag.Error = errorMessage;
            HttpContext.Session.Remove("ErrorMessage");
        }

        return View();
    }
}
