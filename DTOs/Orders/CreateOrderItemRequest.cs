using System.ComponentModel.DataAnnotations;

namespace Gestao_FDC.DTOs.Orders;

public class CreateOrderItemRequest
{
    [Range(1, int.MaxValue)]
    public int ProductId { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    [StringLength(250)]
    public string? Notes { get; set; }
}
