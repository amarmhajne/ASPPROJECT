using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SalesManagement.Data;
using SalesManagement.Helpers;
using SalesManagement.Models;

namespace SalesManagement.Controllers;

[AuthFilter(Role = "Admin")]
public class InventoryController : Controller
{
    private readonly DbConnectionFactory _db;

    public InventoryController(DbConnectionFactory db)
    {
        _db = db;
    }

    [HttpGet]
    public IActionResult Index(string? search)
    {
        using var conn = _db.CreateConnection();
        IEnumerable<Inventory> inventory;

        if (!string.IsNullOrWhiteSpace(search))
        {
            inventory = conn.Query<Inventory>(
                @"SELECT i.*, p.ProductName FROM Inventory i
                  JOIN Products p ON i.ProductId = p.ProductId
                  WHERE p.ProductName LIKE @Search",
                new { Search = $"%{search}%" });
            ViewBag.Search = search;
        }
        else
        {
            inventory = conn.Query<Inventory>(
                @"SELECT i.*, p.ProductName FROM Inventory i
                  JOIN Products p ON i.ProductId = p.ProductId");
        }

        return View(inventory);
    }

    [HttpGet]
    public IActionResult Create()
    {
        LoadProducts();
        return View(new Inventory());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Inventory inventory)
    {
        if (!ModelState.IsValid)
        {
            LoadProducts();
            return View(inventory);
        }

        using var conn = _db.CreateConnection();
        conn.Execute(
            @"INSERT INTO Inventory (ProductId, Quantity, MinQuantity)
              VALUES (@ProductId, @Quantity, @MinQuantity)",
            inventory);

        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        using var conn = _db.CreateConnection();
        var inventory = conn.QueryFirstOrDefault<Inventory>(
            @"SELECT i.*, p.ProductName FROM Inventory i
              JOIN Products p ON i.ProductId = p.ProductId
              WHERE i.InventoryId = @Id", new { Id = id });

        if (inventory == null)
            return NotFound();

        LoadProducts();
        return View(inventory);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, Inventory inventory)
    {
        if (!ModelState.IsValid)
        {
            LoadProducts();
            return View(inventory);
        }

        inventory.InventoryId = id;

        using var conn = _db.CreateConnection();
        conn.Execute(
            @"UPDATE Inventory SET ProductId=@ProductId, Quantity=@Quantity,
              MinQuantity=@MinQuantity WHERE InventoryId=@InventoryId",
            inventory);

        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        using var conn = _db.CreateConnection();
        conn.Execute("DELETE FROM Inventory WHERE InventoryId = @Id", new { Id = id });
        return RedirectToAction("Index");
    }

    private void LoadProducts()
    {
        using var conn = _db.CreateConnection();
        var products = conn.Query<Product>("SELECT * FROM Products WHERE IsActive = TRUE");
        ViewBag.Products = new SelectList(products, "ProductId", "ProductName");
    }
}
