using denicestbankportal.Database;
using denicestbankportal.Models;

namespace denicestbankportal.Logic;

public class LoanService
{
    private readonly LoanProvider _loanProvider;
    private readonly TransactionProvider _transactionProvider_;

    public LoanService(LoanProvider loanProvider, TransactionProvider transactionProvider)
    {
        _loanProvider = loanProvider;
        _transactionProvider_ = transactionProvider;
    }

    public async Task<LoanOverview> GetLoanAsync(Guid loanId)
    {
        var loanWithPersons = await _loanProvider.GetLoanByIdAsync(loanId);
        var loanLatestState = await _transactionProvider_.GetLoanLatestState(loanId);

        return new LoanOverview(loanWithPersons, loanLatestState.TotalTransacted);
    }

    public async Task<IEnumerable<LoanOverview>> GetAllLoansAsync()
    {
        var loansWithPersons = await _loanProvider.GetAllLoansAsync();
        var loansLatestStates = (await _transactionProvider_.GetAllLoansLatestStates()).ToList();

        var results = new List<LoanOverview>();

        foreach (var lwp in loansWithPersons)
        {
            var lls = loansLatestStates.ToList().FirstOrDefault(x => x.LoanId.Equals(lwp.Loan.Id));
            results.Add(new LoanOverview(lwp, lls == null ? 0 : (Int32)lls.TotalTransacted));
        }

        return results;
    }
    
    
    public async Task<IEnumerable<LoanOverview>> GetAllLoansByPersonGuidAsync(Guid personId)
    {
        var loansWithPersons = await _loanProvider.GetLoansByPersonIdAsync(personId);
        var loansLatestStates = (await _transactionProvider_.GetAllLoansLatestStates()).ToList();

        var results = new List<LoanOverview>();

        foreach (var lwp in loansWithPersons)
        {
            var lls = loansLatestStates.ToList().FirstOrDefault(x => x.LoanId.Equals(lwp.Loan.Id));
            results.Add(new LoanOverview(lwp, lls == null ? 0 : (Int32)lls.TotalTransacted));
        }

        return results;
    }

    public async Task<Loan> UpdateLoanAsync(Loan loan)
    {
        return await _loanProvider.UpdateLoanAsync(loan);
    }
    
    public async Task<Boolean> ApproveLoanAsync(Guid loanId)
    {
        var loan = await _loanProvider.GetLoanByIdAsync(loanId);
        if (loan.Loan != null)
        {
            return await _loanProvider.ApproveLoanAsync(loanId);
        }
        throw new KeyNotFoundException($"No loan with Id{loanId}");
    }

    public async Task DeleteLoanAsync(Guid loanId)
    {
        await _loanProvider.DeleteLoanAsync(loanId);
    }

    public async Task<Loan> CreateLoanAsync(LoanCreateObj loanCreateObj)
    {
        var loanDb = new Loan();
        loanDb.Init(loanCreateObj.Loan);
        
        // Add the loan to the provider
        return await _loanProvider.CreateLoanAsync(loanDb, loanCreateObj.Guids);
    }
}