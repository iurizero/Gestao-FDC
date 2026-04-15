using System.ComponentModel.DataAnnotations;
using Gestao_FDC.Models.Enums;

namespace Gestao_FDC.DTOs.Financial;

public class CreateFinancialTransactionRequest
{
    [Range(typeof(decimal), "0.01", "9999999")]
    public decimal Amount { get; set; }

    [Required]
    public TransactionType Type { get; set; }

    [Required]
    [StringLength(200)]
    public string Description { get; set; } = string.Empty;

    [StringLength(100)]
    public string? Category { get; set; }

    public DateTime? TransactionDate { get; set; }
}
