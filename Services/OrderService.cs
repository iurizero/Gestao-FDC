using Gestao_FDC.Configuration;
using Gestao_FDC.Data;
using Gestao_FDC.DTOs.Orders;
using Gestao_FDC.Interfaces;
using Gestao_FDC.Models;
using Gestao_FDC.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Text;
using System.Web;

namespace Gestao_FDC.Services;

public class OrderService : IOrderService
{
    private readonly AppDbContext _context;
    private readonly IRepository<Order> _orderRepo;
    private readonly IRepository<Product> _productRepo;
    private readonly IRepository<FinancialTransaction> _transactionRepo;
    private readonly IRepository<Customer> _customerRepo;
    private readonly BusinessSettings _businessSettings;

    public OrderService(
        AppDbContext context,
        IRepository<Order> orderRepo,
        IRepository<Product> productRepo,
        IRepository<FinancialTransaction> transactionRepo,
        IRepository<Customer> customerRepo,
        IOptions<BusinessSettings> businessOptions)
    {
        _context = context;
        _orderRepo = orderRepo;
        _productRepo = productRepo;
        _transactionRepo = transactionRepo;
        _customerRepo = customerRepo;
        _businessSettings = businessOptions.Value;
    }

    public async Task<Order> CreateOrderAsync(CreateOrderRequest request)
    {
        if (request.Items.Count == 0)
        {
            throw new InvalidOperationException("O pedido deve conter ao menos um item.");
        }

        var order = new Order
        {
            CustomerId = request.CustomerId,
            Notes = request.Notes,
            OrderType = request.OrderType,
            TableNumber = request.TableNumber,
            Items = request.Items.Select(item => new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                Notes = item.Notes
            }).ToList()
        };

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            decimal total = 0;
            foreach (var item in order.Items)
            {
                if (item.Quantity <= 0)
                {
                    throw new InvalidOperationException("A quantidade dos itens deve ser maior que zero.");
                }

                // Usando AsTracking para garantir que o EF rastreie a mudança de estoque
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null)
                {
                    throw new InvalidOperationException($"Produto {item.ProductId} não encontrado.");
                }

                item.UnitPrice = product.Price;
                total += item.Quantity * product.Price;

                if (product.TrackStock)
                {
                    if (product.StockQuantity < item.Quantity)
                    {
                        throw new InvalidOperationException($"Estoque insuficiente para o produto '{product.Name}'. Disponível: {product.StockQuantity}");
                    }

                    product.StockQuantity -= item.Quantity;
                    // O _context.Update não é necessário se usarmos FindAsync e SaveChanges no final do contexto
                }
            }

            order.Total = total;
            order.Status = OrderStatus.Pendente;
            order.OrderDate = DateTime.UtcNow; // Usar UtcNow para consistência

            await _context.Orders.AddAsync(order);

            // Se for pago imediatamente (PDV), cria a transação financeira
            if (order.OrderType != OrderType.Mesa)
            {
                await CreateFinancialTransaction(order);
            }

            if (order.CustomerId.HasValue)
            {
                var customer = await _context.Customers.FindAsync(order.CustomerId.Value);
                if (customer != null)
                {
                    customer.LastOrderDate = DateTime.UtcNow;
                }
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return order;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<Order?> GetOrderByIdAsync(int id) =>
        await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == id);

    public async Task<IEnumerable<Order>> GetAllOrdersAsync() =>
        await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .ToListAsync();

    public async Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus status)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
        if (order == null) return false;

        if (order.Status == status) return true;

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
            var itemName = item.Product?.Name ?? $"Produto {item.ProductId}";
            sb.AppendLine($"{item.Quantity}x - {itemName} - R$ {item.UnitPrice:N2}");

            if (!string.IsNullOrWhiteSpace(item.Notes))
            {
                sb.AppendLine($"  Obs. item: {item.Notes}");
            }
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
        return $"https://wa.me/{_businessSettings.WhatsAppNumber}?text={encodedMessage}";
    }

    private async Task CreateFinancialTransaction(Order order)
    {
        var transactionExists = await _context.FinancialTransactions
            .AnyAsync(t => t.OrderId == order.Id && t.Type == TransactionType.Receita);

        if (transactionExists)
        {
            return;
        }

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
