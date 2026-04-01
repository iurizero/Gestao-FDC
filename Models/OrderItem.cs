using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Gestao_FDC.Models;

public class OrderItem : BaseEntity
{
    public int OrderId { get; set; }
    
    [ForeignKey("OrderId")]
    [JsonIgnore]
    public Order? Order { get; set; }
    
    public int ProductId { get; set; }
    
    [ForeignKey("ProductId")]
    public Product? Product { get; set; }
    
    public int Quantity { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }
    
    public string? Notes { get; set; }
    
    public decimal SubTotal => Quantity * UnitPrice;
}
