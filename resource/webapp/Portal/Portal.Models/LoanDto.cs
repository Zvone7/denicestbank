namespace Portal.Models;

public class LoanDto
{
    public Guid Id { get; set; }
    public Decimal LoanBaseAmount { get; set; }
    public DateTime StartDatetimeUtc { get; set; }
    public Int32 DurationInDays { get; set; }
    public Decimal Interest { get; set; }
    public Decimal LoanTotalAmount { get; set; }
    public Boolean IsApproved { get; set; }
    public String Purpose { get; set; }
    public LoanDto()
    {

    }

    public void Init(LoanBm l, DateTime? utcNow = null)
    {
        StartDatetimeUtc = utcNow ?? DateTime.UtcNow;

        DurationInDays = l.Interest switch
        {
            0.03M => 365 * 15,
            0.04M => 365 * 20,
            _ => 365 * 25
        };

        LoanBaseAmount = l.LoanBaseAmount;
        Interest = l.Interest;
        LoanTotalAmount = CalculateLoanTotalAmount();
        IsApproved = false;
        Purpose = l.Purpose;
    }

    private Decimal CalculateLoanTotalAmount()
    {
        var totalAmount = LoanBaseAmount;
        for (Int32 i = 0; i < DurationInDays; i += 365)
        {
            var yearlyInterest = totalAmount * Interest;
            totalAmount += yearlyInterest;
        }
        return totalAmount;
    }
}