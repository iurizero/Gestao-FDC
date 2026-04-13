using Gestao_FDC.Models;
using Gestao_FDC.Models.Enums;
using Microsoft.AspNetCore.Identity;

namespace Gestao_FDC.Data;

public static class DataSeeder
{
    public static void Seed(AppDbContext context)
    {
        SeedDefaultAdmin(context);

        if (context.Categories.Any()) return;

        var categories = new List<Category>
        {
            new Category { Name = "Salgados", Description = "Salgados fritos e assados" },
            new Category { Name = "Bebidas", Description = "Sucos, refrigerantes e águas" },
            new Category { Name = "Doces", Description = "Sobremesas e doces variados" },
            new Category { Name = "Lanches", Description = "Hambúrgueres e sanduíches" }
        };

        context.Categories.AddRange(categories);
        context.SaveChanges();

        var products = new List<Product>
        {
            new Product { Name = "Coxinha de Frango", Price = 5.50m, CategoryId = categories[0].Id, TrackStock = true, StockQuantity = 50 },
            new Product { Name = "Kibe Recheado", Price = 6.00m, CategoryId = categories[0].Id, TrackStock = true, StockQuantity = 30 },
            new Product { Name = "Coca-Cola 350ml", Price = 5.00m, CategoryId = categories[1].Id, TrackStock = true, StockQuantity = 100 },
            new Product { Name = "Suco de Laranja 400ml", Price = 7.50m, CategoryId = categories[1].Id },
            new Product { Name = "Brigadeiro Gourmet", Price = 4.50m, CategoryId = categories[2].Id, TrackStock = true, StockQuantity = 40 },
            new Product { Name = "X-Burguer Clássico", Price = 18.90m, CategoryId = categories[3].Id }
        };

        context.Products.AddRange(products);
        context.SaveChanges();

        if (!context.InventoryItems.Any())
        {
            var inventory = new List<InventoryItem>
            {
                new InventoryItem { Name = "Farinha de Trigo", Quantity = 10, Unit = "kg", MinQuantity = 2, UnitCost = 4.50m },
                new InventoryItem { Name = "Óleo de Soja", Quantity = 5, Unit = "l", MinQuantity = 1, UnitCost = 7.20m },
                new InventoryItem { Name = "Frango Desfiado", Quantity = 3, Unit = "kg", MinQuantity = 1, UnitCost = 15.00m }
            };
            context.InventoryItems.AddRange(inventory);
            context.SaveChanges();
        }
    }

    private static void SeedDefaultAdmin(AppDbContext context)
    {
        if (context.Users.Any())
        {
            return;
        }

        var admin = new User
        {
            Username = "admin",
            FullName = "Administrador",
            Role = UserRole.Admin,
            IsActive = true
        };

        var passwordHasher = new PasswordHasher<User>();
        admin.PasswordHash = passwordHasher.HashPassword(admin, "Admin@123");

        context.Users.Add(admin);
        context.SaveChanges();
    }
}
