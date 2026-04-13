using Gestao_FDC.Models;
using Gestao_FDC.Models.Enums;

namespace Gestao_FDC.Interfaces;

public interface IFinancialService
{
    Task<decimal> GetDailyRevenueAsync(DateTime date);
    Task<decimal> GetMonthlyRevenueAsync(int month, int year);
    Task<object> GetFinancialSummaryAsync(DateTime start, DateTime end);
    Task<decimal> GetMonthlyExpensesAsync(int month, int year);
}
