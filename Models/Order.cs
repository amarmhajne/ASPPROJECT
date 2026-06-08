using System.ComponentModel.DataAnnotations;

namespace SalesManagement.Models;

public class Order
{
    public int OrderId { get; set; }

    [Required(ErrorMessage = "שדה חובה")]
    [Display(Name = "לקוח")]
    public int CustomerId { get; set; }

    [Required(ErrorMessage = "שדה חובה")]
    [Display(Name = "עובד")]
    public int EmployeeId { get; set; }

    [Display(Name = "תאריך הזמנה")]
    public DateTime? OrderDate { get; set; }

    [Display(Name = "סטטוס")]
    public string Status { get; set; } = "Pending";

    [Display(Name = "סכום כולל")]
    [DisplayFormat(DataFormatString = "{0:N2}")]
    public decimal? TotalAmount { get; set; }

    [Display(Name = "הערות")]
    public string? Notes { get; set; }

    // Navigation display
    [Display(Name = "שם לקוח")]
    public string? CustomerName { get; set; }

    [Display(Name = "שם עובד")]
    public string? EmployeeName { get; set; }

    // Order items
    public List<OrderItem>? Items { get; set; }
}
