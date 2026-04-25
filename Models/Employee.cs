using System.ComponentModel.DataAnnotations;

namespace SalesManagement.Models;

public class Employee
{
    public int EmployeeId { get; set; }

    [Required(ErrorMessage = "שדה חובה")]
    [Display(Name = "שם פרטי")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "שדה חובה")]
    [Display(Name = "שם משפחה")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "שדה חובה")]
    [Display(Name = "שם משתמש")]
    public string Username { get; set; } = string.Empty;

    [Display(Name = "סיסמה")]
    public string? PasswordHash { get; set; }

    [Required(ErrorMessage = "שדה חובה")]
    [Display(Name = "תפקיד")]
    public string Role { get; set; } = "Sales";

    [Display(Name = "טלפון")]
    public string? Phone { get; set; }

    [Display(Name = "אימייל")]
    [EmailAddress(ErrorMessage = "כתובת אימייל לא תקינה")]
    public string? Email { get; set; }

    [Display(Name = "תאריך העסקה")]
    [DataType(DataType.Date)]
    public DateTime? HireDate { get; set; }

    [Display(Name = "פעיל")]
    public bool IsActive { get; set; } = true;

    [Display(Name = "שם מלא")]
    public string FullName => $"{FirstName} {LastName}";
}
