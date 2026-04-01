using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Gestao_FDC.Models;

public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }
    
    public int CategoryId { get; set; }
    
    [ForeignKey("CategoryId")]
    public Category? Category { get; set; }

    public int StockQuantity { get; set; } = 0;
    public bool TrackStock { get; set; } = false;
    
    public string? ImageUrl { get; set; }
}
