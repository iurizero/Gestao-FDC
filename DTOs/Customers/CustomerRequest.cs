using System.ComponentModel.DataAnnotations;

namespace Gestao_FDC.DTOs.Customers;

public class CustomerRequest
{
    [Required]
    [StringLength(120)]
    public string Name { get; set; } = string.Empty;

    [Phone]
    [StringLength(20)]
    public string? Phone { get; set; }

    [StringLength(250)]
    public string? Address { get; set; }

    [EmailAddress]
    [StringLength(120)]
    public string? Email { get; set; }
}
