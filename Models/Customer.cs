using System.ComponentModel.DataAnnotations;

namespace SalesManagement.Models;

public class Customer
{
    public int CustomerId { get; set; }

    [Required(ErrorMessage = "שדה חובה")]
    [Display(Name = "שם פרטי")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "שדה חובה")]
    [Display(Name = "שם משפחה")]
    public string LastName { get; set; } = string.Empty;

    [Display(Name = "טלפון")]
    public string? Phone { get; set; }

    [Display(Name = "אימייל")]
    [EmailAddress(ErrorMessage = "כתובת אימייל לא תקינה")]
    public string? Email { get; set; }

    [Display(Name = "כתובת")]
    public string? Address { get; set; }

    [Display(Name = "תאריך יצירה")]
    [DataType(DataType.Date)]
    public DateTime? CreatedDate { get; set; }

    [Display(Name = "פעיל")]
    public bool IsActive { get; set; } = true;

    [Display(Name = "שם מלא")]
    public string FullName => $"{FirstName} {LastName}";
}
