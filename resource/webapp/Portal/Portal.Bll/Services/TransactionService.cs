using Microsoft.Extensions.Logging;
using Portal.Core.Extensions;
using Portal.Core.Generation;
using Portal.Core.Providers;
using Portal.Core.Services;
using Portal.Models;

namespace Portal.Bll.Services
{

    public class TransactionService : ITransactionService
    {
        private readonly ITransactionProvider _transactionProvider_;
        private readonly ILoanProvider _loanProvider_;
        private readonly IRandomGenerator _randomGenerator_;
        private readonly ILogger<TransactionService> _logger_;

        public TransactionService(
            ITransactionProvider transactionProvider,
            ILoanProvider loanProvider,
            IRandomGenerator randomGenerator,
            ILogger<TransactionService> logger
        )
        {
            _transactionProvider_ = transactionProvider;
            _loanProvider_ = loanProvider;
            _randomGenerator_ = randomGenerator;
            _logger_ = logger;
        }

        public async Task<IEnumerable<TransactDto>> GenerateTransactionsAsync()
        {
            try
            {
                var loanStates = (await _transactionProvider_.GetAllLoansLatestStates()).ToList();
                var loansWithPersonsInfos = await _loanProvider_.GetAllLoansWithPersonsAsync();
                var targets = new List<TransactDto>();
                foreach (var loansWithPersonsInfo in loansWithPersonsInfos)
                {
                    var loan = loansWithPersonsInfo.LoanDto;
                    var loanLatestState = loanStates.FirstOrDefault(x => x.LoanId.Equals(loan.Id));

                    if (!loan.IsApproved) continue;
                    if (loan.IsPastDue()) continue;


                    if (loanLatestState == null || !loan.ShouldAcceptMorePayments(loanLatestState.TotalTransacted))
                    {
                        targets.AddRange(loansWithPersonsInfo
                            .Persons
                            .Select(loanPerson => new TransactDto()
                            {
                                LoanId = loan.Id,
                                PersonId = loanPerson.Id,
                                Amount = _randomGenerator_.GenerateRandomPaymentAmount(loan.LoanTotalAmount)
                            }));
                    }
                }

                var transactResults = new List<TransactDto>();
                foreach (var transact in targets)
                {
                    transactResults.Add(await _transactionProvider_.InsertTransactionAsync(transact));
                }

                return transactResults;
            }
            catch (Exception e)
            {
                _logger_.LogError(e, $"Exception on {nameof(GenerateTransactionsAsync)}: {e.Message}");
                return new List<TransactDto>();
            }
        }


        public async Task<IEnumerable<PaymentVm>> GetLatestPayments(Int32 pageIndex, Int32 pageSize)
        {
            try
            {
                var payments =
                    (await _transactionProvider_.GetAllEnrinchedTransactions())
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize);

                return payments;
            }
            catch (Exception e)
            {
                _logger_.LogError(e, $"Exception on {nameof(GetLatestPayments)}.");
                return new List<PaymentVm>();
            }
        }
    }
}