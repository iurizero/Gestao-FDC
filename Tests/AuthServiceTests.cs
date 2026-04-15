using Gestao_FDC.Configuration;
using Gestao_FDC.Data;
using Gestao_FDC.DTOs.Auth;
using Gestao_FDC.Models;
using Gestao_FDC.Models.Enums;
using Gestao_FDC.Services;
using Gestao_FDC.Tests.Support;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Xunit;

namespace Gestao_FDC.Tests;

public class AuthServiceTests
{
    [Fact]
    public async Task LoginAsync_ReturnsToken_WhenCredentialsAreValid()
    {
        using var factory = new TestDbContextFactory();
        await using var context = factory.CreateContext();

        var passwordHasher = new PasswordHasher<User>();
        var user = new User
        {
            Username = "admin",
            FullName = "Administrador",
            Role = UserRole.Admin,
            IsActive = true
        };
        user.PasswordHash = passwordHasher.HashPassword(user, "Admin@123");

        context.Users.Add(user);
        await context.SaveChangesAsync();

        var service = new AuthService(
            new Repository<User>(context),
            passwordHasher,
            Options.Create(new JwtSettings
            {
                Key = "GestaoFDC-Tests-Key-1234567890-Longer-Key",
                Issuer = "Tests",
                Audience = "Tests",
                ExpiresInMinutes = 60
            }));

        var response = await service.LoginAsync(new LoginRequest
        {
            Username = "admin",
            Password = "Admin@123"
        });

        Assert.NotNull(response);
        Assert.False(string.IsNullOrWhiteSpace(response!.Token));
        Assert.Equal("admin", response.User.Username);
    }

    [Fact]
    public async Task RegisterAsync_Throws_WhenUsernameAlreadyExists()
    {
        using var factory = new TestDbContextFactory();
        await using var context = factory.CreateContext();

        var existingUser = new User
        {
            Username = "caixa",
            FullName = "Operador",
            PasswordHash = "hash",
            Role = UserRole.Atendente,
            IsActive = true
        };

        context.Users.Add(existingUser);
        await context.SaveChangesAsync();

        var service = new AuthService(
            new Repository<User>(context),
            new PasswordHasher<User>(),
            Options.Create(new JwtSettings
            {
                Key = "GestaoFDC-Tests-Key-1234567890-Longer-Key",
                Issuer = "Tests",
                Audience = "Tests",
                ExpiresInMinutes = 60
            }));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.RegisterAsync(new RegisterUserRequest
            {
                Username = "caixa",
                Password = "Senha@123",
                FullName = "Outro Operador"
            }));
    }
}
