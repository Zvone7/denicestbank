using LanguageExt.Common;
using Portal.Models;

namespace Portal.Core.Services;

public interface ILoanService
{
    Task<Result<IEnumerable<LoanOverview>>> GetAllLoansOverviewAsync();
    Task<Result<IEnumerable<LoanOverview>>> GetAllLoansByPersonIdAsync(Guid personId);
    Task<Result<Boolean>> ApproveLoanAsync(Guid loanId);
    Task<Result<LoanDto>> ApplyForLoanAsync(LoanApplication loanApplication);
}