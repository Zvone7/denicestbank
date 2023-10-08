using Portal.Models;

namespace Portal.Core.Providers;

public interface ITransactionProvider
{
    Task<IEnumerable<LoanLatestState>> GetAllLoansLatestStates();
    Task<TransactDto> InsertTransactionAsync(TransactDto transaction);
    Task<IEnumerable<PaymentVm>> GetAllEnrinchedTransactions();
}