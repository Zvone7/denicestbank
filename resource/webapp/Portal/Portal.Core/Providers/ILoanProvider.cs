using LanguageExt.Common;
using Portal.Models;

namespace Portal.Core.Providers;

public interface ILoanProvider
{
    Task<Result<LoanDto>> GetLoanById(Guid loanId);
    Task<Result<List<LoanWithPersons>>> GetAllLoansWithPersonsAsync();
    Task<Result<LoanWithPersons>> GetLoanWithPersonsByIdAsync(Guid loanId);
    Task<Result<IEnumerable<LoanWithPersons>>> GetLoansByPersonIdAsync(Guid personId);
    Task<Result<LoanDto>> CreateLoanAsync(LoanDto loanDto, IEnumerable<Guid> personIds);
    Task<Result<Boolean>> SetLoanToApprovedAsync(Guid loanId, Guid executorId);
}