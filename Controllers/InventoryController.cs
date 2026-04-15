using Gestao_FDC.DTOs.Inventory;
using Gestao_FDC.Interfaces;
using Gestao_FDC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gestao_FDC.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
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
    public async Task<ActionResult<InventoryItem>> Create(InventoryItemRequest request)
    {
        try
        {
            var item = new InventoryItem
            {
                Name = request.Name.Trim(),
                Quantity = request.Quantity,
                Unit = request.Unit.Trim(),
                MinQuantity = request.MinQuantity,
                UnitCost = request.UnitCost
            };

            await _repository.AddAsync(item);
            
            return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro interno: {ex.Message}");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, InventoryItemRequest request)
    {
        var item = await _repository.GetByIdAsync(id);
        if (item == null) return NotFound();

        item.Name = request.Name.Trim();
        item.Quantity = request.Quantity;
        item.Unit = request.Unit.Trim();
        item.MinQuantity = request.MinQuantity;
        item.UnitCost = request.UnitCost;
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
