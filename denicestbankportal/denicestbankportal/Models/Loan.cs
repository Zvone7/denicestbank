namespace denicestbankportal.Models;

public class Loan
{
    public Guid Id { get; set; }
    public decimal LoanBaseAmount { get; set; }
    public DateTime StartDatetimeUtc { get; set; }
    public decimal Interest { get; set; }
    public decimal LoanTotalAmount { get; set; }
    public bool IsApproved { get; set; }
}