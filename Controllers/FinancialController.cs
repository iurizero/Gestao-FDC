using Gestao_FDC.DTOs.Financial;
using Gestao_FDC.Interfaces;
using Gestao_FDC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gestao_FDC.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class FinancialController : ControllerBase
{
    private readonly IFinancialService _financialService;
    private readonly IRepository<FinancialTransaction> _repository;

    public FinancialController(
        IFinancialService financialService,
        IRepository<FinancialTransaction> repository)
    {
        _financialService = financialService;
        _repository = repository;
    }

    [HttpGet("transactions")]
    public async Task<ActionResult<IEnumerable<FinancialTransaction>>> GetTransactions()
        => Ok(await _repository.GetAllAsync());

    [HttpPost("transactions")]
    public async Task<ActionResult<FinancialTransaction>> CreateTransaction(CreateFinancialTransactionRequest request)
    {
        var transaction = new FinancialTransaction
        {
            Amount = request.Amount,
            Type = request.Type,
            Description = request.Description.Trim(),
            Category = request.Category?.Trim(),
            TransactionDate = request.TransactionDate ?? DateTime.Now
        };

        await _repository.AddAsync(transaction);
        return CreatedAtAction(nameof(GetTransactions), new { id = transaction.Id }, transaction);
    }

    [HttpGet("summary")]
    public async Task<ActionResult<object>> GetSummary([FromQuery] DateTime start, [FromQuery] DateTime end)
    {
        if (start == default) start = DateTime.Now.AddDays(-30);
        if (end == default) end = DateTime.Now;

        var summary = await _financialService.GetFinancialSummaryAsync(start, end);
        return Ok(summary);
    }

    [HttpGet("daily-revenue")]
    public async Task<ActionResult<decimal>> GetDailyRevenue([FromQuery] DateTime date)
    {
        if (date == default) date = DateTime.Now;
        var revenue = await _financialService.GetDailyRevenueAsync(date);
        return Ok(new { date, revenue });
    }
}
