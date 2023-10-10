using LanguageExt.Common;
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

        public async Task<Result<TransactDto>> GenerateTransactionAsync(Guid personId, Guid loanId, Guid executorId)
        {
            try
            {
                var loanResult = await _loanService_.GetLoanById(loanId);
                var transactionCreateResult = await loanResult.MatchAsync(async loanDto =>
                {
                    var transact = new TransactDto()
                    {
                        Amount = _randomGenerator_.GenerateRandomPaymentAmount(loanDto.LoanTotalAmount),
                        CreatedBy = executorId,
                        CreatedDatetimeUtc = DateTime.UtcNow,
                        LoanId = loanId,
                        PersonId = personId
                    };

                    var transactionCreateResult2 = await _transactionProvider_.InsertTransactionAsync(transact);
                    return transactionCreateResult2;
                });
                return transactionCreateResult;
            }
            catch (Exception e)
            {
                _logger_.LogError(e, $"Exception on {nameof(GenerateTransactionsAsync)}: {e.Message}");
                return new Result<TransactDto>();
            }
        }

        public async Task<Result<IEnumerable<TransactDto>>> GenerateTransactionsAsync()
        {
            try
            {
                var loansWithPersonsInfosResult = await _loanService_.GetAllLoansOverviewAsync();
                var allTransactionsInsertResult =
                    await loansWithPersonsInfosResult.BindAsync<IEnumerable<LoanOverview>, IEnumerable<TransactDto>>(
                        async loansWithPersonsInfos =>
                        {
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
                                            CreatedDatetimeUtc = DateTime.UtcNow,
                                            Amount = _randomGenerator_.GenerateRandomPaymentAmount(loan.LoanTotalAmount)
                                        }));
                                }
                            }

                            var transactResults = new List<TransactDto>();
                            foreach (var transact in targets)
                            {
                                var transactionInsertResult = await _transactionProvider_.InsertTransactionAsync(transact);
                                var transaction = transactionInsertResult.Match(
                                    transactDto => transactDto,
                                    fail =>
                                    {
                                        // explicitly swallow exception
                                        _logger_.LogError(fail, "Exception inserting transaction.");
                                        return null!;
                                    });
                                if (transaction != null)
                                {
                                    _logger_.LogInformation($"Inserted transaction on loan {transact.LoanId} by person {transact.PersonId}");
                                    transactResults.Add(transaction);
                                }
                            }

                            return transactResults;
                        });
                return allTransactionsInsertResult;
            }
            catch (Exception e)
            {
                _logger_.LogError(e, $"Exception on {nameof(GenerateTransactionsAsync)}: {e.Message}");
                return new Result<IEnumerable<TransactDto>>();
            }
        }

        public async Task<Result<List<PaymentVm>>> GetLatestPaymentsAsync(Int32 pageIndex, Int32 pageSize)
        {
            try
            {
                var enrichedTransactionsResult = await _transactionProvider_.GetAllEnrichedTransactions();
                var pageOfEnrichedTransactionsResult = enrichedTransactionsResult.Bind(_ =>
                {
                    return enrichedTransactionsResult.Match(x =>
                        new Result<List<PaymentVm>>(x
                            .Skip((pageIndex - 1) * pageSize)
                            .Take(pageSize)
                            .ToList()));
                });
                return pageOfEnrichedTransactionsResult;
            }
            catch (Exception e)
            {
                _logger_.LogError(e, $"Exception on {nameof(GetLatestPaymentsAsync)}.");
                return new Result<List<PaymentVm>>(e);
            }
        }
    }
}