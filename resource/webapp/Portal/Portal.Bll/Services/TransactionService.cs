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
        private readonly ILoanService _loanService_;
        private readonly IRandomGenerator _randomGenerator_;
        private readonly ILogger<ITransactionService> _logger_;

        public TransactionService(
            ITransactionProvider transactionProvider,
            ILoanService loanService,
            IRandomGenerator randomGenerator,
            ILogger<ITransactionService> logger
        )
        {
            _transactionProvider_ = transactionProvider;
            _loanService_ = loanService;
            _randomGenerator_ = randomGenerator;
            _logger_ = logger;
        }

        public async Task<IEnumerable<TransactDto>> GenerateTransactionsAsync()
        {
            try
            {
                var loansWithPersonsInfos = await _loanService_.GetAllLoansOverviewAsync();
                var targets = new List<TransactDto>();
                foreach (var loansWithPersonsInfo in loansWithPersonsInfos)
                {
                    var loan = loansWithPersonsInfo.LoanDto;
                    if (!loan.IsApproved) continue;
                    if (loan.IsPastDue()) continue;
                    
                    if (loan.ShouldAcceptMorePayments(loansWithPersonsInfo.LoanTotalReturned))
                    {
                        targets.AddRange(loansWithPersonsInfo
                            .Persons
                            .Select(loanPerson => new TransactDto()
                            {
                                LoanId = loan.Id,
                                PersonId = loanPerson.Id,
                                UpdateDatetimeUtc = DateTime.UtcNow,
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

        public async Task<IEnumerable<PaymentVm>> GetLatestPaymentsAsync(Int32 pageIndex, Int32 pageSize)
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
                _logger_.LogError(e, $"Exception on {nameof(GetLatestPaymentsAsync)}.");
                return new List<PaymentVm>();
            }
        }
    }
}