using System.ComponentModel.DataAnnotations;
using Gestao_FDC.Models.Enums;

namespace Gestao_FDC.DTOs.Orders;

public class CreateOrderRequest
{
    [Required]
    public OrderType OrderType { get; set; }

    public int? TableNumber { get; set; }

    public int? CustomerId { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }

    [MinLength(1)]
    public List<CreateOrderItemRequest> Items { get; set; } = new();
}
