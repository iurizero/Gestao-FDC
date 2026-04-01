using System.ComponentModel.DataAnnotations.Schema;
using Gestao_FDC.Models.Enums;

namespace Gestao_FDC.Models;

public class Order : BaseEntity
{
    public DateTime OrderDate { get; set; } = DateTime.Now;
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal Total { get; set; }
    
    public OrderType OrderType { get; set; }
    public OrderStatus Status { get; set; }
    
    public int? TableNumber { get; set; }
    
    public int? CustomerId { get; set; }
    
    [ForeignKey("CustomerId")]
    public Customer? Customer { get; set; }

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    
    public string? Notes { get; set; }
}
