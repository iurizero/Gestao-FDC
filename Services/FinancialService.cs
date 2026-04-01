using Gestao_FDC.Interfaces;
using Gestao_FDC.Models;
using Gestao_FDC.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Gestao_FDC.Services;

public class FinancialService : IFinancialService
{
    private readonly IRepository<FinancialTransaction> _transactionRepo;

    public FinancialService(IRepository<FinancialTransaction> transactionRepo)
    {
        _transactionRepo = transactionRepo;
    }

    public async Task<decimal> GetDailyRevenueAsync(DateTime date)
    {
        var transactions = await _transactionRepo.GetAllAsync();
        return transactions
            .Where(t => t.TransactionDate.Date == date.Date && t.Type == TransactionType.Receita)
            .Sum(t => t.Amount);
    }

    public async Task<decimal> GetMonthlyRevenueAsync(int month, int year)
    {
        var transactions = await _transactionRepo.GetAllAsync();
        return transactions
            .Where(t => t.TransactionDate.Month == month && t.TransactionDate.Year == year && t.Type == TransactionType.Receita)
            .Sum(t => t.Amount);
    }

    public async Task<object> GetFinancialSummaryAsync(DateTime start, DateTime end)
    {
        var transactions = await _transactionRepo.GetAllAsync();
        var periodTransactions = transactions.Where(t => t.TransactionDate >= start && t.TransactionDate <= end).ToList();

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
