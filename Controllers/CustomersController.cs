using Gestao_FDC.Interfaces;
using Gestao_FDC.Models;
using Microsoft.AspNetCore.Mvc;

namespace Gestao_FDC.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly IRepository<Customer> _repository;

    public CustomersController(IRepository<Customer> repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Customer>>> GetAll() => Ok(await _repository.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<Customer>> GetById(int id)
    {
        var customer = await _repository.GetByIdAsync(id);
        if (customer == null) return NotFound();
        return Ok(customer);
    }

    [HttpPost]
    public async Task<ActionResult<Customer>> Create(Customer customer)
    {
        await _repository.AddAsync(customer);
        return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customer);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Customer customer)
    {
        if (id != customer.Id) return BadRequest();
        await _repository.UpdateAsync(customer);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _repository.DeleteAsync(id);
        return NoContent();
    }
}
