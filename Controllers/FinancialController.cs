using Gestao_FDC.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Gestao_FDC.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FinancialController : ControllerBase
{
    private readonly IFinancialService _financialService;

    public FinancialController(IFinancialService financialService)
    {
        _financialService = financialService;
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
