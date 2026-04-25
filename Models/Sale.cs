using System.ComponentModel.DataAnnotations;

namespace SalesManagement.Models;

public class Sale
{
    public int SaleId { get; set; }

    [Required(ErrorMessage = "שדה חובה")]
    [Display(Name = "הזמנה")]
    public int OrderId { get; set; }

    [Required(ErrorMessage = "שדה חובה")]
    [Display(Name = "עובד")]
    public int EmployeeId { get; set; }

    [Display(Name = "תאריך מכירה")]
    public DateTime? SaleDate { get; set; }

    [Required(ErrorMessage = "שדה חובה")]
    [Display(Name = "סכום ששולם")]
    [Range(0.01, double.MaxValue, ErrorMessage = "סכום חייב להיות חיובי")]
    public decimal AmountPaid { get; set; }

    [Required(ErrorMessage = "שדה חובה")]
    [Display(Name = "אמצעי תשלום")]
    public string PaymentMethod { get; set; } = "Cash";

    // Navigation display
    [Display(Name = "מספר הזמנה")]
    public string? OrderDisplay { get; set; }

    [Display(Name = "שם עובד")]
    public string? EmployeeName { get; set; }
}
