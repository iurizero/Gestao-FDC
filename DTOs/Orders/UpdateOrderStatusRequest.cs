using System.ComponentModel.DataAnnotations;
using Gestao_FDC.Models.Enums;

namespace Gestao_FDC.DTOs.Orders;

public class UpdateOrderStatusRequest
{
    [Required]
    public OrderStatus Status { get; set; }
}
