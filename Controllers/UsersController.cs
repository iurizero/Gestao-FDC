using Gestao_FDC.Interfaces;
using Gestao_FDC.Models;
using Microsoft.AspNetCore.Mvc;

namespace Gestao_FDC.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IRepository<User> _repository;

    public UsersController(IRepository<User> repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetAll() => Ok(await _repository.GetAllAsync());

    [HttpPost]
    public async Task<ActionResult<User>> Create(User user)
    {
        // Nota: Em um sistema real, aqui teríamos hashing de senha
        await _repository.AddAsync(user);
        return Ok(user);
    }
}
