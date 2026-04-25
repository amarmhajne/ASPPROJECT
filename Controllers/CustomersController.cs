using Dapper;
using Microsoft.AspNetCore.Mvc;
using SalesManagement.Data;
using SalesManagement.Helpers;
using SalesManagement.Models;

namespace SalesManagement.Controllers;

[AuthFilter(Role = "Admin,Sales")]
public class CustomersController : Controller
{
    private readonly DbConnectionFactory _db;

    public CustomersController(DbConnectionFactory db)
    {
        _db = db;
    }

    [HttpGet]
    public IActionResult Index(string? search)
    {
        using var conn = _db.CreateConnection();
        IEnumerable<Customer> customers;

        if (!string.IsNullOrWhiteSpace(search))
        {
            customers = conn.Query<Customer>(
                "SELECT * FROM Customers WHERE FirstName LIKE @Search OR LastName LIKE @Search OR Phone LIKE @Search",
                new { Search = $"%{search}%" });
            ViewBag.Search = search;
        }
        else
        {
            customers = conn.Query<Customer>("SELECT * FROM Customers");
        }

        return View(customers);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new Customer());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Customer customer)
    {
        if (!ModelState.IsValid)
            return View(customer);

        using var conn = _db.CreateConnection();
        conn.Execute(
            @"INSERT INTO Customers (FirstName, LastName, Phone, Email, Address, IsActive)
              VALUES (@FirstName, @LastName, @Phone, @Email, @Address, @IsActive)",
            customer);

        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        using var conn = _db.CreateConnection();
        var customer = conn.QueryFirstOrDefault<Customer>(
            "SELECT * FROM Customers WHERE CustomerId = @Id", new { Id = id });

        if (customer == null)
            return NotFound();

        return View(customer);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Customer customer)
    {
        if (!ModelState.IsValid)
            return View(customer);

        customer.CustomerId = id;

        using var conn = _db.CreateConnection();
        conn.Execute(
            @"UPDATE Customers SET FirstName=@FirstName, LastName=@LastName,
              Phone=@Phone, Email=@Email, Address=@Address, IsActive=@IsActive
              WHERE CustomerId=@CustomerId",
            customer);

        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        using var conn = _db.CreateConnection();
        conn.Execute("UPDATE Customers SET IsActive = FALSE WHERE CustomerId = @Id", new { Id = id });
        return RedirectToAction("Index");
    }
}
