using denicestbankportal.Database;
using denicestbankportal.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace denicestbankportal.Logic
{
    public class TransactionService
    {
        private readonly TransactionProvider _transactionProvider;

        public TransactionService(TransactionProvider transactionProvider)
        {
            _transactionProvider = transactionProvider;
        }

        public async Task<IEnumerable<LoanLatestState>> GetAllLoansLatestStates()
        {
            return await _transactionProvider.GetAllLoansLatestStates();
        }
        public async Task<LoanLatestState> GetLoansLatestStatesByLoanId(Guid loanId)
        {
            return await _transactionProvider.GetLoanLatestState(loanId);
        }

        public async Task<Transact> InsertTransactionAsync(Transact transaction)
        {
            return await _transactionProvider.InsertTransactionAsync(transaction);
        }
    }
}