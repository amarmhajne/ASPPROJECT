using System.Security.Cryptography;
using System.Text;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using SalesManagement.Data;
using SalesManagement.Helpers;
using SalesManagement.Models;

namespace SalesManagement.Controllers;

[AuthFilter(Role = "Admin")]
public class EmployeesController : Controller
{
    private readonly DbConnectionFactory _db;

    public EmployeesController(DbConnectionFactory db)
    {
        _db = db;
    }

    [HttpGet]
    public IActionResult Index(string? search)
    {
        using var conn = _db.CreateConnection();
        IEnumerable<Employee> employees;

        if (!string.IsNullOrWhiteSpace(search))
        {
            employees = conn.Query<Employee>(
                "SELECT * FROM Employees WHERE FirstName LIKE @Search OR LastName LIKE @Search OR Username LIKE @Search",
                new { Search = $"%{search}%" });
            ViewBag.Search = search;
        }
        else
        {
            employees = conn.Query<Employee>("SELECT * FROM Employees");
        }

        return View(employees);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new Employee());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Employee employee, string? password)
    {
        if (!ModelState.IsValid)
            return View(employee);

        if (string.IsNullOrWhiteSpace(password))
        {
            ModelState.AddModelError("PasswordHash", "נא להזין סיסמה");
            return View(employee);
        }

        employee.PasswordHash = ComputeSha256(password);

        using var conn = _db.CreateConnection();
        conn.Execute(
            @"INSERT INTO Employees (FirstName, LastName, Username, PasswordHash, Role, Phone, Email, HireDate, IsActive)
              VALUES (@FirstName, @LastName, @Username, @PasswordHash, @Role, @Phone, @Email, @HireDate, @IsActive)",
            employee);

        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        using var conn = _db.CreateConnection();
        var employee = conn.QueryFirstOrDefault<Employee>(
            "SELECT * FROM Employees WHERE EmployeeId = @Id", new { Id = id });

        if (employee == null)
            return NotFound();

        return View(employee);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Employee employee, string? password)
    {
        if (!ModelState.IsValid)
            return View(employee);

        employee.EmployeeId = id;

        using var conn = _db.CreateConnection();

        if (!string.IsNullOrWhiteSpace(password))
        {
            employee.PasswordHash = ComputeSha256(password);
            conn.Execute(
                @"UPDATE Employees SET FirstName=@FirstName, LastName=@LastName, Username=@Username,
                  PasswordHash=@PasswordHash, Role=@Role, Phone=@Phone, Email=@Email,
                  HireDate=@HireDate, IsActive=@IsActive WHERE EmployeeId=@EmployeeId",
                employee);
        }
        else
        {
            conn.Execute(
                @"UPDATE Employees SET FirstName=@FirstName, LastName=@LastName, Username=@Username,
                  Role=@Role, Phone=@Phone, Email=@Email, HireDate=@HireDate,
                  IsActive=@IsActive WHERE EmployeeId=@EmployeeId",
                employee);
        }

        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        using var conn = _db.CreateConnection();
        conn.Execute("UPDATE Employees SET IsActive = FALSE WHERE EmployeeId = @Id", new { Id = id });
        return RedirectToAction("Index");
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
