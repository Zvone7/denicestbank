using LanguageExt.Common;
using Portal.Models;

namespace Portal.Core.Services;

public interface ITransactionService
{
    Task<Result<IEnumerable<TransactDto>>> GenerateTransactionsAsync();
    Task<Result<List<PaymentVm>>> GetLatestPaymentsAsync(Int32 pageIndex, Int32 pageSize);
}