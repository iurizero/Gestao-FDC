using Gestao_FDC.Interfaces;
using Gestao_FDC.Models;
using Gestao_FDC.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Web;

namespace Gestao_FDC.Services;

public class OrderService : IOrderService
{
    private readonly IRepository<Order> _orderRepo;
    private readonly IRepository<Product> _productRepo;
    private readonly IRepository<FinancialTransaction> _transactionRepo;
    private readonly IRepository<Customer> _customerRepo;

    public OrderService(
        IRepository<Order> orderRepo,
        IRepository<Product> productRepo,
        IRepository<FinancialTransaction> transactionRepo,
        IRepository<Customer> customerRepo)
    {
        _orderRepo = orderRepo;
        _productRepo = productRepo;
        _transactionRepo = transactionRepo;
        _customerRepo = customerRepo;
    }

    public async Task<Order> CreateOrderAsync(Order order)
    {
        decimal total = 0;
        foreach (var item in order.Items)
        {
            var product = await _productRepo.GetByIdAsync(item.ProductId);
            if (product != null)
            {
                item.UnitPrice = product.Price;
                total += item.Quantity * product.Price;

                if (product.TrackStock)
                {
                    product.StockQuantity -= item.Quantity;
                    await _productRepo.UpdateAsync(product);
                }
            }
        }

        order.Total = total;
        order.Status = OrderStatus.Pendente;
        order.OrderDate = DateTime.Now;

        await _orderRepo.AddAsync(order);

        // Se for pago imediatamente (PDV), cria a transação financeira
        if (order.OrderType != OrderType.Mesa)
        {
            await CreateFinancialTransaction(order);
        }

        if (order.CustomerId.HasValue)
        {
            var customer = await _customerRepo.GetByIdAsync(order.CustomerId.Value);
            if (customer != null)
            {
                customer.LastOrderDate = DateTime.Now;
                await _customerRepo.UpdateAsync(customer);
            }
        }

        return order;
    }

    public async Task<Order?> GetOrderByIdAsync(int id) => await _orderRepo.GetByIdAsync(id);

    public async Task<IEnumerable<Order>> GetAllOrdersAsync() => await _orderRepo.GetAllAsync();

    public async Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus status)
    {
        var order = await _orderRepo.GetByIdAsync(orderId);
        if (order == null) return false;

        order.Status = status;
        await _orderRepo.UpdateAsync(order);

        // Se o pedido de mesa for finalizado (Entregue), registra o financeiro
        if (order.OrderType == OrderType.Mesa && status == OrderStatus.Entregue)
        {
            await CreateFinancialTransaction(order);
        }

        return true;
    }

    public string GenerateWhatsAppMessage(Order order)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"*Novo Pedido - {order.OrderType}*");
        sb.AppendLine($"Data: {order.OrderDate:dd/MM/yyyy HH:mm}");
        sb.AppendLine("----------------------------");

        foreach (var item in order.Items)
        {
            // Nota: Em um cenário real, precisaríamos garantir que o Product está carregado
            sb.AppendLine($"{item.Quantity}x - {item.Notes ?? "Item"} - R$ {item.UnitPrice:N2}");
        }

        sb.AppendLine("----------------------------");
        sb.AppendLine($"*Total: R$ {order.Total:N2}*");
        
        if (order.Customer != null)
        {
            sb.AppendLine($"\n*Cliente:* {order.Customer.Name}");
            sb.AppendLine($"*Telefone:* {order.Customer.Phone}");
            sb.AppendLine($"*Endereço:* {order.Customer.Address}");
        }

        if (!string.IsNullOrEmpty(order.Notes))
        {
            sb.AppendLine($"\n*Obs:* {order.Notes}");
        }

        string encodedMessage = HttpUtility.UrlEncode(sb.ToString());
        return $"https://wa.me/5599999999999?text={encodedMessage}"; // Placeholder para o número do estabelecimento
    }

    private async Task CreateFinancialTransaction(Order order)
    {
        var transaction = new FinancialTransaction
        {
            Amount = order.Total,
            Type = TransactionType.Receita,
            TransactionDate = DateTime.Now,
            Description = $"Venda - Pedido #{order.Id}",
            OrderId = order.Id,
            Category = "Venda"
        };
        await _transactionRepo.AddAsync(transaction);
    }
}
