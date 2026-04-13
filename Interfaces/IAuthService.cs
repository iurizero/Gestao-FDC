using Gestao_FDC.DTOs.Auth;

namespace Gestao_FDC.Interfaces;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
    Task<UserResponse> RegisterAsync(RegisterUserRequest request);
}
