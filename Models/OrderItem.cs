using System.ComponentModel.DataAnnotations;

namespace SalesManagement.Models;

public class OrderItem
{
    public int OrderItemId { get; set; }

    [Required(ErrorMessage = "שדה חובה")]
    [Display(Name = "הזמנה")]
    public int OrderId { get; set; }

    [Required(ErrorMessage = "שדה חובה")]
    [Display(Name = "מוצר")]
    public int ProductId { get; set; }

    [Required(ErrorMessage = "שדה חובה")]
    [Display(Name = "כמות")]
    [Range(1, int.MaxValue, ErrorMessage = "כמות חייבת להיות לפחות 1")]
    public int Quantity { get; set; }

    [Required(ErrorMessage = "שדה חובה")]
    [Display(Name = "מחיר יחידה")]
    public decimal UnitPrice { get; set; }

    // Navigation display
    [Display(Name = "שם מוצר")]
    public string? ProductName { get; set; }

    [Display(Name = "סה\"כ")]
    public decimal Total => Quantity * UnitPrice;
}
