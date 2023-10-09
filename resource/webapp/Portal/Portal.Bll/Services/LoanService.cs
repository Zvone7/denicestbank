using Microsoft.Extensions.Logging;
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

    public async Task<IEnumerable<LoanOverview>> GetAllLoansOverviewAsync()
    {
        try
        {
            var loansWithPersons = await _loanProvider_.GetAllLoansWithPersonsAsync();
            var loansLatestStates = (await _transactionProvider_.GetAllLoansLatestStates()).ToList();

            var results = new List<LoanOverview>();

            foreach (var lwp in loansWithPersons)
            {
                var lls = loansLatestStates.ToList().FirstOrDefault(x => x.LoanId.Equals(lwp.LoanDto.Id));
                results.Add(new LoanOverview(lwp, lls == null ? 0 : (Int32)lls.TotalTransacted));
            }

            return results;
        }
        catch (Exception e)
        {
            _logger_.LogError(e, $"Exception on {nameof(GetAllLoansOverviewAsync)}");
            return new List<LoanOverview>();
        }
    }
    
    public async Task<IEnumerable<LoanOverview>> GetAllLoansByPersonIdAsync(Guid personId)
    {
        try
        {
            var loansWithPersons = await _loanProvider_.GetLoansByPersonIdAsync(personId);
            var loansLatestStates = (await _transactionProvider_.GetAllLoansLatestStates()).ToList();

            var results = new List<LoanOverview>();

            foreach (var lwp in loansWithPersons)
            {
                var lls = loansLatestStates.ToList().FirstOrDefault(x => x.LoanId.Equals(lwp.LoanDto.Id));
                results.Add(new LoanOverview(lwp, lls == null ? 0 : (Int32)lls.TotalTransacted));
            }

            return results;
        }
        catch (Exception e)
        {
            _logger_.LogError(e, $"Exception on {nameof(GetAllLoansByPersonIdAsync)}");
            return new List<LoanOverview>();
        }
    }

    public async Task<Boolean> ApproveLoanAsync(Guid loanId)
    {
        try
        {
            var loan = await _loanProvider_.GetLoanWithPersonsByIdAsync(loanId);
            if (loan.LoanDto != null)
            {
                return await _loanProvider_.SetLoanToApprovedAsync(loanId);
            }
            throw new KeyNotFoundException($"No loan with Id{loanId}");
        }
        catch (Exception e)
        {
            _logger_.LogError(e, $"Exception on {nameof(ApproveLoanAsync)}");
            return false;
        }
    }

    public async Task<LoanDto?> ApplyForLoanAsync(LoanApplication loanApplication)
    {
        try
        {
            var persons = new List<PersonDto>();
            foreach (var personId in loanApplication.Guids)
            {
                var person = await _personService_.GetPersonByIdAsync(personId);
                if (person != null) persons.Add(person);
            }

            var loanDb = new LoanDto();
            loanDb.Init(loanApplication.Loan);

            // Add the loan to the provider
            return await _loanProvider_.CreateLoanAsync(loanDb, persons.Select(x => x.Id));
        }
        catch (Exception e)
        {
            _logger_.LogError(e, $"Exception on {nameof(GetAllLoansOverviewAsync)}");
            return null;
        }
    }
}