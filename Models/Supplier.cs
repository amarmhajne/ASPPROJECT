using System.ComponentModel.DataAnnotations;

namespace SalesManagement.Models;

public class Supplier
{
    public int SupplierId { get; set; }

    [Required(ErrorMessage = "שדה חובה")]
    [Display(Name = "שם חברה")]
    public string CompanyName { get; set; } = string.Empty;

    [Display(Name = "איש קשר")]
    public string? ContactName { get; set; }

    [Display(Name = "טלפון")]
    public string? Phone { get; set; }

    [Display(Name = "אימייל")]
    [EmailAddress(ErrorMessage = "כתובת אימייל לא תקינה")]
    public string? Email { get; set; }

    [Display(Name = "כתובת")]
    public string? Address { get; set; }

    [Display(Name = "פעיל")]
    public bool IsActive { get; set; } = true;
}
