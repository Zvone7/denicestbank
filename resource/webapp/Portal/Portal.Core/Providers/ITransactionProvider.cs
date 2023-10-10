using LanguageExt.Common;
using Portal.Models;

namespace Portal.Core.Providers;

public interface ITransactionProvider
{
    Task<Result<IEnumerable<LoanLatestState>>> GetAllLoansLatestStates();
    Task<Result<TransactDto>> InsertTransactionAsync(TransactDto transaction);
    Task<Result<IEnumerable<PaymentVm>>> GetAllEnrichedTransactions();
}