using LanguageExt.Common;
using Portal.Models;

namespace Portal.Core.Services;

public interface ILoanService
{
    Task<Result<LoanDto>> GetLoanById(Guid loanId);
    Task<Result<IEnumerable<LoanOverview>>> GetAllLoansOverviewAsync();
    Task<Result<IEnumerable<LoanOverview>>> GetAllLoansByPersonIdAsync(Guid personId);
    Task<Result<Boolean>> ApproveLoanAsync(Guid loanId, Guid executorId);
    Task<Result<LoanDto>> ApplyForLoanAsync(LoanApplication loanApplication);
}