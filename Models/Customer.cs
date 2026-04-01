namespace Gestao_FDC.Models;

public class Customer : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? Email { get; set; }
    public DateTime? LastOrderDate { get; set; }
}
