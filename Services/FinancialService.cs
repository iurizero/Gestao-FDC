using Gestao_FDC.Data;
using Gestao_FDC.Interfaces;
using Gestao_FDC.Models;
using Gestao_FDC.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Gestao_FDC.Services;

public class FinancialService : IFinancialService
{
    private readonly AppDbContext _context;

    public FinancialService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<decimal> GetDailyRevenueAsync(DateTime date)
        => (await _context.FinancialTransactions
            .Where(t => t.TransactionDate.Date == date.Date && t.Type == TransactionType.Receita)
            .SumAsync(t => (decimal?)t.Amount)) ?? 0m;

    public async Task<decimal> GetMonthlyRevenueAsync(int month, int year)
        => (await _context.FinancialTransactions
            .Where(t => t.TransactionDate.Month == month && t.TransactionDate.Year == year && t.Type == TransactionType.Receita)
            .SumAsync(t => (decimal?)t.Amount)) ?? 0m;

    public async Task<decimal> GetMonthlyExpensesAsync(int month, int year)
        => (await _context.FinancialTransactions
            .Where(t => t.TransactionDate.Month == month && t.TransactionDate.Year == year && t.Type == TransactionType.Despesa)
            .SumAsync(t => (decimal?)t.Amount)) ?? 0m;

    public async Task<object> GetFinancialSummaryAsync(DateTime start, DateTime end)
    {
        var periodTransactions = await _context.FinancialTransactions
            .Where(t => t.TransactionDate >= start && t.TransactionDate <= end)
            .ToListAsync();

        var totalRevenue = periodTransactions.Where(t => t.Type == TransactionType.Receita).Sum(t => t.Amount);
        var totalExpenses = periodTransactions.Where(t => t.Type == TransactionType.Despesa).Sum(t => t.Amount);

        return new
        {
            TotalRevenue = totalRevenue,
            TotalExpenses = totalExpenses,
            Balance = totalRevenue - totalExpenses,
            TransactionsCount = periodTransactions.Count
        };
    }
}
