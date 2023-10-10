using Portal.Models;

namespace Portal.Core.Extensions;

public static class LoanExtensions
{
    private const Decimal LOAN_PAID_OFF_LIMIT = 0.9m;
    public static Boolean IsPastDue(this LoanDto loanDto, DateTime? datetimeUtcNow = null)
    {
        var utcNow = datetimeUtcNow ?? DateTime.UtcNow;
        var loanExpiryDatetime = loanDto.StartDatetimeUtc.AddDays(loanDto.DurationInDays);
        return utcNow >= loanExpiryDatetime;
    }
    public static Boolean ShouldAcceptMorePayments(this LoanDto loanDto, Decimal loanTotalReturned, Decimal paidOffLimitPercent = LOAN_PAID_OFF_LIMIT)
    {
        return loanTotalReturned < paidOffLimitPercent * loanDto.LoanTotalAmount;
    }
}