using Gestao_FDC.Interfaces;
using Gestao_FDC.Models;
using Microsoft.AspNetCore.Mvc;

namespace Gestao_FDC.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly IRepository<Category> _repository;

    public CategoriesController(IRepository<Category> repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Category>>> GetAll() => Ok(await _repository.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<Category>> GetById(int id)
    {
        var category = await _repository.GetByIdAsync(id);
        if (category == null) return NotFound();
        return Ok(category);
    }

    [HttpPost]
    public async Task<ActionResult<Category>> Create(Category category)
    {
        await _repository.AddAsync(category);
        return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Category category)
    {
        if (id != category.Id) return BadRequest();
        await _repository.UpdateAsync(category);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _repository.DeleteAsync(id);
        return NoContent();
    }
}
