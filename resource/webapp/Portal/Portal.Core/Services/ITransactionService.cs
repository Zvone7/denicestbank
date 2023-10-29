using LanguageExt.Common;
using Portal.Models;

namespace Portal.Core.Services;

public interface ITransactionService
{
    public Task<Result<TransactDto>> GenerateTransactionAsync(Guid personId, Guid loanId, Guid executorId);
    Task<Result<IEnumerable<TransactDto>>> GenerateTransactionsAsync();
    Task<Result<List<PaymentVm>>> GetLatestPaymentsAsync(Int32 pageIndex, Int32 pageSize);
}