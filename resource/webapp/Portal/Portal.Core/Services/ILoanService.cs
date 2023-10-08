using Portal.Models;

namespace Portal.Core.Services;

public interface ILoanService
{
    Task<IEnumerable<LoanOverview>> GetAllLoansOverviewAsync();
    Task<IEnumerable<LoanOverview>> GetAllLoansByPersonIdAsync(Guid personId);
    Task<Boolean> ApproveLoanAsync(Guid loanId);
    Task<LoanDto?> ApplyForLoanAsync(LoanApplication loanApplication);
}