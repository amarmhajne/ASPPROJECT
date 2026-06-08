using Dapper;
using Microsoft.AspNetCore.Mvc;
using SalesManagement.Data;
using SalesManagement.Helpers;
using SalesManagement.Models;

namespace SalesManagement.Controllers;

[AuthFilter(Role = "Admin")]
public class SuppliersController : Controller
{
    private readonly DbConnectionFactory _db;

    public SuppliersController(DbConnectionFactory db)
    {
        _db = db;
    }

    [HttpGet]
    public IActionResult Index(string? search)
    {
        using var conn = _db.CreateConnection();
        IEnumerable<Supplier> suppliers;

        if (!string.IsNullOrWhiteSpace(search))
        {
            suppliers = conn.Query<Supplier>(
                "SELECT * FROM Suppliers WHERE CompanyName LIKE @Search OR ContactName LIKE @Search",
                new { Search = $"%{search}%" });
            ViewBag.Search = search;
        }
        else
        {
            suppliers = conn.Query<Supplier>("SELECT * FROM Suppliers");
        }

        return View(suppliers);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new Supplier());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Supplier supplier)
    {
        if (!ModelState.IsValid)
            return View(supplier);

        using var conn = _db.CreateConnection();
        conn.Execute(
            @"INSERT INTO Suppliers (CompanyName, ContactName, Phone, Email, Address, IsActive)
              VALUES (@CompanyName, @ContactName, @Phone, @Email, @Address, @IsActive)",
            supplier);

        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        using var conn = _db.CreateConnection();
        var supplier = conn.QueryFirstOrDefault<Supplier>(
            "SELECT * FROM Suppliers WHERE SupplierId = @Id", new { Id = id });

        if (supplier == null)
            return NotFound();

        return View(supplier);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Supplier supplier)
    {
        if (!ModelState.IsValid)
            return View(supplier);

        supplier.SupplierId = id;

        using var conn = _db.CreateConnection();
        conn.Execute(
            @"UPDATE Suppliers SET CompanyName=@CompanyName, ContactName=@ContactName,
              Phone=@Phone, Email=@Email, Address=@Address, IsActive=@IsActive
              WHERE SupplierId=@SupplierId",
            supplier);

        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        using var conn = _db.CreateConnection();
        conn.Execute("UPDATE Suppliers SET IsActive = FALSE WHERE SupplierId = @Id", new { Id = id });
        return RedirectToAction("Index");
    }
}
