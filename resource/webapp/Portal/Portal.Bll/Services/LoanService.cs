using LanguageExt.Common;
using Microsoft.Extensions.Logging;
using Portal.Core.Extensions;
using Portal.Core.Providers;
using Portal.Core.Services;
using Portal.Models;

namespace Portal.Bll.Services;

public class LoanService : ILoanService
{
    private readonly ILoanProvider _loanProvider_;
    private readonly IPersonService _personService_;
    private readonly ITransactionProvider _transactionProvider_;
    private readonly ILogger<ILoanService> _logger_;

    public LoanService(
        ILoanProvider loanProvider,
        IPersonService personService,
        ITransactionProvider transactionProvider,
        ILogger<ILoanService> logger
    )
    {
        _loanProvider_ = loanProvider;
        _personService_ = personService;
        _transactionProvider_ = transactionProvider;
        _logger_ = logger;
    }

    public async Task<Result<IEnumerable<LoanOverview>>> GetAllLoansOverviewAsync()
    {
        try
        {
            var loansWithPersonsResult = await _loanProvider_.GetAllLoansWithPersonsAsync();
            var loansOverviewsResult2 = await loansWithPersonsResult.BindAsync(async loansWithPersons =>
            {

                var loansLatestStatesResult = (await _transactionProvider_.GetAllLoansLatestStates());
                var loanOverviewsResult = loansLatestStatesResult.Bind(loanLatestStates =>
                {
                    var results = new List<LoanOverview>();
                    var latestStates = loanLatestStates.ToList();

                    foreach (var lwp in loansWithPersons)
                    {
                        var lls = latestStates.FirstOrDefault(x => x.LoanId.Equals(lwp.LoanDto.Id));
                        results.Add(new LoanOverview(
                            lwp,
                            totalReturned: lls == null ? 0 : (Int32)lls.TotalTransacted));
                    }

                    return new Result<IEnumerable<LoanOverview>>(results);
                });

                return loanOverviewsResult;
            });
            return loansOverviewsResult2;
        }
        catch (Exception e)
        {
            _logger_.LogError(e, $"Exception on {nameof(GetAllLoansOverviewAsync)}");
            return new List<LoanOverview>();
        }
    }

    public async Task<Result<IEnumerable<LoanOverview>>> GetAllLoansByPersonIdAsync(Guid personId)
    {
        try
        {
            var loansWithPersonsResult = await _loanProvider_.GetLoansByPersonIdAsync(personId);

            var loanOverviewsResult = await loansWithPersonsResult.BindAsync(async loansWithPersons =>
            {
                var loansLatestStatesResult = (await _transactionProvider_.GetAllLoansLatestStates());
                var loanOverviewsResult = loansLatestStatesResult.Bind(loansLatestStatesSucc =>
                {
                    var results = new List<LoanOverview>();
                    var loanLatestStates = loansLatestStatesSucc.ToList();

                    foreach (var lwp in loansWithPersons)
                    {
                        var lls = loanLatestStates.ToList().FirstOrDefault(x => x.LoanId.Equals(lwp.LoanDto.Id));
                        results.Add(new LoanOverview(lwp, lls == null ? 0 : (Int32)lls.TotalTransacted));
                    }

                    return new Result<IEnumerable<LoanOverview>>(results);
                });
                return loanOverviewsResult;
            });
            return loanOverviewsResult;
        }
        catch (Exception e)
        {
            _logger_.LogError(e, $"Exception on {nameof(GetAllLoansByPersonIdAsync)}");
            return new List<LoanOverview>();
        }
    }

    public async Task<Result<Boolean>> ApproveLoanAsync(Guid loanId)
    {
        try
        {
            var loanResult = await _loanProvider_.GetLoanWithPersonsByIdAsync(loanId);
            var updateLoanResult = await loanResult.BindAsync(async loan =>
            {
                var loanNotNullResult = loan.IsNotNull();
                var setLoanApprovedResult = await loanNotNullResult.MatchAsync(async loanSucc =>
                {
                    var final = await _loanProvider_.SetLoanToApprovedAsync(loanId);
                    return final;
                });
                return setLoanApprovedResult;
            });
            return updateLoanResult;
        }
        catch (Exception e)
        {
            _logger_.LogError(e, $"Exception on {nameof(ApproveLoanAsync)}");
            return new Result<Boolean>(e);
        }
    }

    public async Task<Result<LoanDto>> ApplyForLoanAsync(LoanApplication loanApplication)
    {
        try
        {
            var persons = new List<PersonDto>();
            foreach (var personId in loanApplication.Guids)
            {
                var personResult = await _personService_.GetPersonByIdAsync(personId);
                var person = personResult.Match(p => { return p; },
                    _ => null!);
                if (person != null) persons.Add(person);
            }

            var loanDb = new LoanDto();
            loanDb.Init(loanApplication.Loan);

            return await _loanProvider_.CreateLoanAsync(loanDb, persons.Select(x => x.Id));
        }
        catch (Exception e)
        {
            _logger_.LogError(e, $"Exception on {nameof(GetAllLoansOverviewAsync)}");
            return null;
        }
    }
}