using System.ComponentModel.DataAnnotations;

namespace SalesManagement.Models;

public class Product
{
    public int ProductId { get; set; }

    [Required(ErrorMessage = "שדה חובה")]
    [Display(Name = "שם מוצר")]
    public string ProductName { get; set; } = string.Empty;

    [Display(Name = "ספק")]
    public int? SupplierId { get; set; }

    [Display(Name = "קטגוריה")]
    public string? Category { get; set; }

    [Required(ErrorMessage = "שדה חובה")]
    [Display(Name = "מחיר יחידה")]
    [Range(0.01, double.MaxValue, ErrorMessage = "מחיר חייב להיות חיובי")]
    public decimal UnitPrice { get; set; }

    [Display(Name = "פעיל")]
    public bool IsActive { get; set; } = true;

    // Navigation display
    [Display(Name = "שם ספק")]
    public string? SupplierName { get; set; }
}
