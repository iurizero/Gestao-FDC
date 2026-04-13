using Gestao_FDC.Models.Enums;

namespace Gestao_FDC.DTOs.Auth;

public class RegisterUserRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Atendente;
    public bool IsActive { get; set; } = true;
}
