using Gestao_FDC.DTOs.Auth;
using Gestao_FDC.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gestao_FDC.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class UsersController : ControllerBase
{
    private readonly IRepository<Gestao_FDC.Models.User> _repository;
    private readonly IAuthService _authService;

    public UsersController(IRepository<Gestao_FDC.Models.User> repository, IAuthService authService)
    {
        _repository = repository;
        _authService = authService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserResponse>>> GetAll()
    {
        var users = await _repository.GetAllAsync();
        return Ok(users.Select(user => new UserResponse
        {
            Id = user.Id,
            Username = user.Username,
            FullName = user.FullName,
            Role = user.Role,
            IsActive = user.IsActive
        }));
    }

    [HttpPost]
    public async Task<ActionResult<UserResponse>> Create(RegisterUserRequest request)
    {
        try
        {
            var user = await _authService.RegisterAsync(request);
            return CreatedAtAction(nameof(GetAll), new { id = user.Id }, user);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
