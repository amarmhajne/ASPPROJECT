using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SalesManagement.Data;
using SalesManagement.Helpers;
using SalesManagement.Models;

namespace SalesManagement.Controllers;

[AuthFilter(Role = "Admin,Sales")]
public class SalesController : Controller
{
    private readonly DbConnectionFactory _db;

    public SalesController(DbConnectionFactory db)
    {
        _db = db;
    }

    [HttpGet]
    public IActionResult Index(string? search)
    {
        using var conn = _db.CreateConnection();
        IEnumerable<Sale> sales;

        if (!string.IsNullOrWhiteSpace(search))
        {
            sales = conn.Query<Sale>(
                @"SELECT s.*, CONCAT('#', s.OrderId) AS OrderDisplay,
                  CONCAT(e.FirstName, ' ', e.LastName) AS EmployeeName
                  FROM Sales s
                  JOIN Employees e ON s.EmployeeId = e.EmployeeId
                  WHERE s.SaleId LIKE @Search OR s.OrderId LIKE @Search",
                new { Search = $"%{search}%" });
            ViewBag.Search = search;
        }
        else
        {
            sales = conn.Query<Sale>(
                @"SELECT s.*, CONCAT('#', s.OrderId) AS OrderDisplay,
                  CONCAT(e.FirstName, ' ', e.LastName) AS EmployeeName
                  FROM Sales s
                  JOIN Employees e ON s.EmployeeId = e.EmployeeId
                  ORDER BY s.SaleDate DESC");
        }

        return View(sales);
    }

    [HttpGet]
    public IActionResult Create()
    {
        LoadDropdowns();
        return View(new Sale());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Sale sale)
    {
        if (!ModelState.IsValid)
        {
            LoadDropdowns();
            return View(sale);
        }

        sale.EmployeeId = HttpContext.Session.GetInt32("UserId") ?? 0;

        using var conn = _db.CreateConnection();
        conn.Execute(
            @"INSERT INTO Sales (OrderId, EmployeeId, AmountPaid, PaymentMethod)
              VALUES (@OrderId, @EmployeeId, @AmountPaid, @PaymentMethod)",
            sale);

        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        using var conn = _db.CreateConnection();
        var sale = conn.QueryFirstOrDefault<Sale>(
            "SELECT * FROM Sales WHERE SaleId = @Id", new { Id = id });

        if (sale == null)
            return NotFound();

        LoadDropdowns();
        return View(sale);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Sale sale)
    {
        if (!ModelState.IsValid)
        {
            LoadDropdowns();
            return View(sale);
        }

        sale.SaleId = id;

        using var conn = _db.CreateConnection();
        conn.Execute(
            @"UPDATE Sales SET OrderId=@OrderId, EmployeeId=@EmployeeId,
              AmountPaid=@AmountPaid, PaymentMethod=@PaymentMethod
              WHERE SaleId=@SaleId",
            sale);

        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        using var conn = _db.CreateConnection();
        conn.Execute("DELETE FROM Sales WHERE SaleId = @Id", new { Id = id });
        return RedirectToAction("Index");
    }

    private void LoadDropdowns()
    {
        using var conn = _db.CreateConnection();
        var orders = conn.Query<Order>("SELECT * FROM Orders WHERE Status != 'Cancelled'");
        ViewBag.Orders = new SelectList(orders.Select(o => new { o.OrderId, Display = $"הזמנה #{o.OrderId}" }),
            "OrderId", "Display");

        var employees = conn.Query<Employee>("SELECT * FROM Employees WHERE IsActive = TRUE");
        ViewBag.Employees = new SelectList(employees.Select(e => new { e.EmployeeId, Name = $"{e.FirstName} {e.LastName}" }),
            "EmployeeId", "Name");

        var paymentMethods = new[] {
            new { Value = "Cash", Text = "מזומן" },
            new { Value = "CreditCard", Text = "כרטיס אשראי" },
            new { Value = "BankTransfer", Text = "העברה בנקאית" }
        };
        ViewBag.PaymentMethods = new SelectList(paymentMethods, "Value", "Text");
    }
}
