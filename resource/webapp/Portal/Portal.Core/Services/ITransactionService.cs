using Portal.Models;

namespace Portal.Core.Services;

public interface ITransactionService
{
    Task<IEnumerable<TransactDto>> GenerateTransactionsAsync();
    Task<IEnumerable<PaymentVm>> GetLatestPaymentsAsync(Int32 pageIndex, Int32 pageSize);
}