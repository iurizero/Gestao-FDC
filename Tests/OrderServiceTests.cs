using Gestao_FDC.Configuration;
using Gestao_FDC.Data;
using Gestao_FDC.DTOs.Orders;
using Gestao_FDC.Models;
using Gestao_FDC.Models.Enums;
using Gestao_FDC.Services;
using Gestao_FDC.Tests.Support;
using Microsoft.Extensions.Options;
using Xunit;

namespace Gestao_FDC.Tests;

public class OrderServiceTests
{
    [Fact]
    public async Task CreateOrderAsync_CreatesRevenueAndUpdatesStock_ForDeliveryOrders()
    {
        using var factory = new TestDbContextFactory();
        await using var context = factory.CreateContext();

        var category = new Category { Name = "Salgados" };
        var product = new Product
        {
            Name = "Coxinha",
            Price = 8.5m,
            Category = category,
            TrackStock = true,
            StockQuantity = 10
        };

        context.Products.Add(product);
        await context.SaveChangesAsync();

        var service = CreateService(context);

        var order = await service.CreateOrderAsync(new CreateOrderRequest
        {
            OrderType = OrderType.Delivery,
            Items =
            [
                new CreateOrderItemRequest
                {
                    ProductId = product.Id,
                    Quantity = 2
                }
            ]
        });

        Assert.Equal(17m, order.Total);
        Assert.Equal(OrderStatus.Pendente, order.Status);
        Assert.Single(context.FinancialTransactions);
        Assert.Equal(8, (await context.Products.FindAsync(product.Id))!.StockQuantity);
    }

    [Fact]
    public async Task UpdateOrderStatusAsync_CreatesRevenue_WhenMesaOrderBecomesDelivered()
    {
        using var factory = new TestDbContextFactory();
        await using var context = factory.CreateContext();

        var category = new Category { Name = "Bebidas" };
        var product = new Product
        {
            Name = "Suco",
            Price = 7m,
            Category = category
        };

        var order = new Order
        {
            OrderType = OrderType.Mesa,
            Status = OrderStatus.Pendente,
            Total = 14m,
            Items =
            [
                new OrderItem
                {
                    Product = product,
                    Quantity = 2,
                    UnitPrice = 7m
                }
            ]
        };

        context.Orders.Add(order);
        await context.SaveChangesAsync();

        var service = CreateService(context);
        var updated = await service.UpdateOrderStatusAsync(order.Id, OrderStatus.Entregue);

        Assert.True(updated);
        Assert.Single(context.FinancialTransactions);
        Assert.Equal(order.Id, context.FinancialTransactions.Single().OrderId);
    }

    private static OrderService CreateService(AppDbContext context) =>
        new(
            context,
            new Repository<Order>(context),
            new Repository<Product>(context),
            new Repository<FinancialTransaction>(context),
            new Repository<Customer>(context),
            Options.Create(new BusinessSettings
            {
                Name = "FDC Lanches",
                WhatsAppNumber = "5581999999999"
            }));
}
