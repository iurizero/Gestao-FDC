using Gestao_FDC.Interfaces;
using Gestao_FDC.Models;
using Microsoft.AspNetCore.Mvc;

namespace Gestao_FDC.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly IRepository<InventoryItem> _repository;

    public InventoryController(IRepository<InventoryItem> repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<InventoryItem>>> GetAll() => Ok(await _repository.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<InventoryItem>> GetById(int id)
    {
        var item = await _repository.GetByIdAsync(id);
        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<InventoryItem>> Create(InventoryItem item)
    {
        await _repository.AddAsync(item);
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, InventoryItem item)
    {
        if (id != item.Id) return BadRequest();
        await _repository.UpdateAsync(item);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _repository.DeleteAsync(id);
        return NoContent();
    }
}
