namespace Portal.Models;

public class LoanApplication
{
    public LoanBm Loan { get; set; }
    public IEnumerable<Guid> Guids { get; set; }
}