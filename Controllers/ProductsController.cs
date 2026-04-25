using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SalesManagement.Data;
using SalesManagement.Helpers;
using SalesManagement.Models;

namespace SalesManagement.Controllers;

[AuthFilter(Role = "Admin")]
public class ProductsController : Controller
{
    private readonly DbConnectionFactory _db;

    public ProductsController(DbConnectionFactory db)
    {
        _db = db;
    }

    [HttpGet]
    public IActionResult Index(string? search)
    {
        using var conn = _db.CreateConnection();
        IEnumerable<Product> products;

        if (!string.IsNullOrWhiteSpace(search))
        {
            products = conn.Query<Product>(
                @"SELECT p.*, s.CompanyName AS SupplierName FROM Products p
                  LEFT JOIN Suppliers s ON p.SupplierId = s.SupplierId
                  WHERE p.ProductName LIKE @Search OR p.Category LIKE @Search",
                new { Search = $"%{search}%" });
            ViewBag.Search = search;
        }
        else
        {
            products = conn.Query<Product>(
                @"SELECT p.*, s.CompanyName AS SupplierName FROM Products p
                  LEFT JOIN Suppliers s ON p.SupplierId = s.SupplierId");
        }

        return View(products);
    }

    [HttpGet]
    public IActionResult Create()
    {
        LoadSuppliers();
        return View(new Product());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Product product)
    {
        if (!ModelState.IsValid)
        {
            LoadSuppliers();
            return View(product);
        }

        using var conn = _db.CreateConnection();
        conn.Execute(
            @"INSERT INTO Products (ProductName, SupplierId, Category, UnitPrice, IsActive)
              VALUES (@ProductName, @SupplierId, @Category, @UnitPrice, @IsActive)",
            product);

        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        using var conn = _db.CreateConnection();
        var product = conn.QueryFirstOrDefault<Product>(
            "SELECT * FROM Products WHERE ProductId = @Id", new { Id = id });

        if (product == null)
            return NotFound();

        LoadSuppliers();
        return View(product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Product product)
    {
        if (!ModelState.IsValid)
        {
            LoadSuppliers();
            return View(product);
        }

        product.ProductId = id;

        using var conn = _db.CreateConnection();
        conn.Execute(
            @"UPDATE Products SET ProductName=@ProductName, SupplierId=@SupplierId,
              Category=@Category, UnitPrice=@UnitPrice, IsActive=@IsActive
              WHERE ProductId=@ProductId",
            product);

        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        using var conn = _db.CreateConnection();
        conn.Execute("UPDATE Products SET IsActive = FALSE WHERE ProductId = @Id", new { Id = id });
        return RedirectToAction("Index");
    }

    private void LoadSuppliers()
    {
        using var conn = _db.CreateConnection();
        var suppliers = conn.Query<Supplier>("SELECT * FROM Suppliers WHERE IsActive = TRUE");
        ViewBag.Suppliers = new SelectList(suppliers, "SupplierId", "CompanyName");
    }
}
