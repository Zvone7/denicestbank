using Portal.Models;

namespace Portal.Core.Providers;

public interface ILoanProvider
{
    Task<IEnumerable<LoanWithPersons>> GetAllLoansWithPersonsAsync();
    Task<LoanWithPersons> GetLoanWithPersonsByIdAsync(Guid loanId);
    Task<IEnumerable<LoanWithPersons>> GetLoansByPersonIdAsync(Guid personId);
    Task<LoanDto> CreateLoanAsync(LoanDto loanDto, IEnumerable<Guid> personIds);
    Task<Boolean> SetLoanToApprovedAsync(Guid loanId);
}