using System.ComponentModel.DataAnnotations.Schema;
using Gestao_FDC.Models.Enums;

namespace Gestao_FDC.Models;

public class FinancialTransaction : BaseEntity
{
    public DateTime TransactionDate { get; set; } = DateTime.Now;
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }
    
    public TransactionType Type { get; set; }
    
    public string Description { get; set; } = string.Empty;
    
    public int? OrderId { get; set; }
    
    [ForeignKey("OrderId")]
    public Order? Order { get; set; }
    
    public string? Category { get; set; } // e.g. "Venda", "Insumos", "Salários", etc.
}
