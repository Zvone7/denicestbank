namespace Portal.Api.Models;

public class LoanApplyObj
{
    public LoanBm Loan { get; set; }
    public IEnumerable<Guid> Guids { get; set; }
}