using denicestbankportal.Database;
using denicestbankportal.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Identity.Client;

namespace denicestbankportal.Logic
{
    public class TransactionService
    {
        private readonly TransactionProvider _transactionProvider;
        private readonly LoanProvider _loanProvider;

        public TransactionService(TransactionProvider transactionProvider, LoanProvider loanProvider)
        {
            _transactionProvider = transactionProvider;
            _loanProvider = loanProvider;
        }

        public async Task<IEnumerable<LoanLatestState>> GetAllLoansLatestStates()
        {
            return await _transactionProvider.GetAllLoansLatestStates();
        }
        public async Task<LoanLatestState> GetLoansLatestStatesByLoanId(Guid loanId)
        {
            return await _transactionProvider.GetLoanLatestState(loanId);
        }

        public async Task<IEnumerable<Transact>> AutoGenerateTransactionsAsync()
        {
            try
            {
                var loanStates = (await _transactionProvider.GetAllLoansLatestStates()).ToList();
                var loansWithPersonsInfos = await _loanProvider.GetAllLoansWithPersonsAsync();
                var targets = new List<Transact>();
                foreach (var loansWithPersonsInfo in loansWithPersonsInfos)
                {
                    var loan = loansWithPersonsInfo.Loan;
                    var loanLatestState = loanStates.FirstOrDefault(x => x.LoanId.Equals(loan.Id));

                    if (!loan.IsApproved) continue;
                    if (!IsWithinTimeLimit(loan.DurationInDays)) continue;


                    if (loanLatestState == null || !LoanIsEnoughPaidOff(loan.LoanTotalAmount, loanLatestState.TotalTransacted))
                    {
                        targets.AddRange(loansWithPersonsInfo.Persons.Select(loanPerson => new Transact() { LoanId = loan.Id, PersonId = loanPerson.Id, Amount = GenerateRandomPaymentAmount(loan.LoanTotalAmount) }));
                    }
                }

                var transactResults = new List<Transact>();
                foreach (var transact in targets)
                {
                    transactResults.Add(await _transactionProvider.InsertTransactionAsync(transact));
                }

                return transactResults;
            }
            catch (Exception e)
            {

                throw;
            }
        }

        private static bool IsWithinTimeLimit(Int32 loanDurationInDays, DateTime? datetimeUtcNow = null)
        {
            var utcNow = datetimeUtcNow ?? DateTime.UtcNow;
            return utcNow < utcNow.AddDays(loanDurationInDays);
        }

        private const decimal LOAN_PAID_OFF_LIMIT = 0.9m;

        private static bool LoanIsEnoughPaidOff(decimal totalAmount, decimal paidOffAmount, decimal paidOffLimitPercent = LOAN_PAID_OFF_LIMIT)
        {
            return paidOffAmount > paidOffLimitPercent * totalAmount;
        }

        public decimal GenerateRandomPaymentAmount(decimal totalAmount)
        {
            var random = new Random();
            decimal minPercentage = 0.0001m;
            decimal maxPercentage = 0.0009m;

            var randomPercentage = (decimal)random.NextDouble() * (maxPercentage - minPercentage) + minPercentage;

            return totalAmount * randomPercentage;
        }
    }


}