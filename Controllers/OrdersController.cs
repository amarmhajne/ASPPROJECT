using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SalesManagement.Data;
using SalesManagement.Helpers;
using SalesManagement.Models;

namespace SalesManagement.Controllers;

[AuthFilter(Role = "Admin,Sales")]
public class OrdersController : Controller
{
    private readonly DbConnectionFactory _db;

    public OrdersController(DbConnectionFactory db)
    {
        _db = db;
    }

    [HttpGet]
    public IActionResult Index(string? search)
    {
        using var conn = _db.CreateConnection();
        IEnumerable<Order> orders;

        if (!string.IsNullOrWhiteSpace(search))
        {
            orders = conn.Query<Order>(
                @"SELECT o.*, CONCAT(c.FirstName, ' ', c.LastName) AS CustomerName,
                  CONCAT(e.FirstName, ' ', e.LastName) AS EmployeeName
                  FROM Orders o
                  JOIN Customers c ON o.CustomerId = c.CustomerId
                  JOIN Employees e ON o.EmployeeId = e.EmployeeId
                  WHERE o.OrderId LIKE @Search OR c.FirstName LIKE @Search OR c.LastName LIKE @Search",
                new { Search = $"%{search}%" });
            ViewBag.Search = search;
        }
        else
        {
            orders = conn.Query<Order>(
                @"SELECT o.*, CONCAT(c.FirstName, ' ', c.LastName) AS CustomerName,
                  CONCAT(e.FirstName, ' ', e.LastName) AS EmployeeName
                  FROM Orders o
                  JOIN Customers c ON o.CustomerId = c.CustomerId
                  JOIN Employees e ON o.EmployeeId = e.EmployeeId
                  ORDER BY o.OrderDate DESC");
        }

        return View(orders);
    }

    [HttpGet]
    public IActionResult Create()
    {
        LoadDropdowns();
        return View(new Order());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Order order)
    {
        if (!ModelState.IsValid)
        {
            LoadDropdowns();
            return View(order);
        }

        order.EmployeeId = HttpContext.Session.GetInt32("UserId") ?? 0;

        using var conn = _db.CreateConnection();
        conn.Execute(
            @"INSERT INTO Orders (CustomerId, EmployeeId, Status, TotalAmount, Notes)
              VALUES (@CustomerId, @EmployeeId, @Status, @TotalAmount, @Notes)",
            order);

        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        using var conn = _db.CreateConnection();
        var order = conn.QueryFirstOrDefault<Order>(
            "SELECT * FROM Orders WHERE OrderId = @Id", new { Id = id });

        if (order == null)
            return NotFound();

        LoadDropdowns();
        return View(order);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Order order)
    {
        if (!ModelState.IsValid)
        {
            LoadDropdowns();
            return View(order);
        }

        order.OrderId = id;

        using var conn = _db.CreateConnection();
        conn.Execute(
            @"UPDATE Orders SET CustomerId=@CustomerId, EmployeeId=@EmployeeId,
              Status=@Status, TotalAmount=@TotalAmount, Notes=@Notes
              WHERE OrderId=@OrderId",
            order);

        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        using var conn = _db.CreateConnection();
        conn.Execute("UPDATE Orders SET Status = 'Cancelled' WHERE OrderId = @Id", new { Id = id });
        return RedirectToAction("Index");
    }

    private void LoadDropdowns()
    {
        using var conn = _db.CreateConnection();
        var customers = conn.Query<Customer>("SELECT * FROM Customers WHERE IsActive = TRUE");
        ViewBag.Customers = new SelectList(customers.Select(c => new { c.CustomerId, Name = $"{c.FirstName} {c.LastName}" }),
            "CustomerId", "Name");

        var employees = conn.Query<Employee>("SELECT * FROM Employees WHERE IsActive = TRUE");
        ViewBag.Employees = new SelectList(employees.Select(e => new { e.EmployeeId, Name = $"{e.FirstName} {e.LastName}" }),
            "EmployeeId", "Name");

        var statuses = new[] { "Pending", "Processing", "Completed", "Cancelled" };
        ViewBag.Statuses = new SelectList(statuses);
    }
}
