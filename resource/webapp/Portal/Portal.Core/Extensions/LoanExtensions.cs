using Portal.Models;

namespace Portal.Core.Extensions;

public static class LoanExtensions
{
    private const decimal LOAN_PAID_OFF_LIMIT = 0.9m;
    public static bool IsPastDue(this LoanDto loanDto, DateTime? datetimeUtcNow = null)
    {
        var utcNow = datetimeUtcNow ?? DateTime.UtcNow;
        var loanExpiryDatetime = loanDto.StartDatetimeUtc.AddDays(loanDto.DurationInDays);
        return utcNow >= loanExpiryDatetime;
    }
    public static bool ShouldAcceptMorePayments(this LoanDto loanDto, decimal loanTotalReturned, decimal paidOffLimitPercent = LOAN_PAID_OFF_LIMIT)
    {
        return loanTotalReturned < paidOffLimitPercent * loanDto.LoanTotalAmount;
    }
}