using System.Security.Cryptography;
using System.Text;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using SalesManagement.Data;
using SalesManagement.Models;

namespace SalesManagement.Controllers;

public class AccountController : Controller
{
    private readonly DbConnectionFactory _db;

    public AccountController(DbConnectionFactory db)
    {
        _db = db;
    }

    [HttpGet]
    public IActionResult Login()
    {
        if (HttpContext.Session.GetInt32("UserId") != null)
            return RedirectToAction("Index", "Dashboard");
        return View();
    }

    [HttpPost]
    public IActionResult Login(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            ViewBag.Error = "נא למלא שם משתמש וסיסמה";
            return View();
        }

        using var conn = _db.CreateConnection();
        var passwordHash = ComputeSha256(password);

        var employee = conn.QueryFirstOrDefault<Employee>(
            "SELECT * FROM Employees WHERE Username = @Username AND PasswordHash = @PasswordHash AND IsActive = TRUE",
            new { Username = username, PasswordHash = passwordHash });

        if (employee == null)
        {
            ViewBag.Error = "שם משתמש או סיסמה שגויים";
            return View();
        }

        HttpContext.Session.SetInt32("UserId", employee.EmployeeId);
        HttpContext.Session.SetString("UserRole", employee.Role);
        HttpContext.Session.SetString("FullName", employee.FullName);

        return RedirectToAction("Index", "Dashboard");
    }

    [HttpGet]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }

    private static string ComputeSha256(string input)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        var sb = new StringBuilder();
        foreach (var b in bytes)
            sb.Append(b.ToString("x2"));
        return sb.ToString();
    }
}
