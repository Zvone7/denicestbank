using Portal.Models;

namespace Portal.Core.Extensions;

public static class LoanExtensions
{
    private const decimal LOAN_PAID_OFF_LIMIT = 0.9m;
    public static bool IsPastDue(this LoanDto loanDto, DateTime? datetimeUtcNow = null)
    {
        var utcNow = datetimeUtcNow ?? DateTime.UtcNow;
        return utcNow > utcNow.AddDays(loanDto.DurationInDays);
    }
    public static bool ShouldAcceptMorePayments(this LoanDto loanDto, decimal paidOffAmount, decimal paidOffLimitPercent = LOAN_PAID_OFF_LIMIT)
    {
        return paidOffAmount > paidOffLimitPercent * loanDto.LoanTotalAmount;
    }
}