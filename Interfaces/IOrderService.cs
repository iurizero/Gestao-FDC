using Gestao_FDC.DTOs.Orders;
using Gestao_FDC.Models;

namespace Gestao_FDC.Interfaces;

public interface IOrderService
{
    Task<Order> CreateOrderAsync(CreateOrderRequest request);
    Task<Order?> GetOrderByIdAsync(int id);
    Task<IEnumerable<Order>> GetAllOrdersAsync();
    Task<bool> UpdateOrderStatusAsync(int orderId, Models.Enums.OrderStatus status);
    string GenerateWhatsAppMessage(Order order);
}
