using System.ComponentModel.DataAnnotations;

namespace SalesManagement.Models;

public class Inventory
{
    public int InventoryId { get; set; }

    [Required(ErrorMessage = "שדה חובה")]
    [Display(Name = "מוצר")]
    public int ProductId { get; set; }

    [Required(ErrorMessage = "שדה חובה")]
    [Display(Name = "כמות")]
    [Range(0, int.MaxValue, ErrorMessage = "כמות חייבת להיות חיובית")]
    public int Quantity { get; set; }

    [Display(Name = "כמות מינימלית")]
    [Range(0, int.MaxValue, ErrorMessage = "כמות חייבת להיות חיובית")]
    public int MinQuantity { get; set; } = 5;

    [Display(Name = "עדכון אחרון")]
    public DateTime? LastUpdated { get; set; }

    // Navigation display
    [Display(Name = "שם מוצר")]
    public string? ProductName { get; set; }
}
