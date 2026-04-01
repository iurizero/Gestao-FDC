using Gestao_FDC.Interfaces;
using Gestao_FDC.Models;
using Microsoft.AspNetCore.Mvc;

namespace Gestao_FDC.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SalgadosController : ControllerBase
{
    private readonly IRepository<Salgado> _repository;

    public SalgadosController(IRepository<Salgado> repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Salgado>>> GetAll()
    {
        var salgados = await _repository.GetAllAsync();
        return Ok(salgados);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Salgado>> GetById(int id)
    {
        var salgado = await _repository.GetByIdAsync(id);
        if (salgado == null) return NotFound();
        return Ok(salgado);
    }

    [HttpPost]
    public async Task<ActionResult<Salgado>> Create(Salgado salgado)
    {
        await _repository.AddAsync(salgado);
        return CreatedAtAction(nameof(GetById), new { id = salgado.Id }, salgado);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Salgado salgado)
    {
        if (id != salgado.Id) return BadRequest();
        await _repository.UpdateAsync(salgado);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _repository.DeleteAsync(id);
        return NoContent();
    }
}
