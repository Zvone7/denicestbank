namespace denicestbankportal.Models;

public class LoanCreateObj
{
    public LoanBm Loan { get; set; }
    public IEnumerable<Guid> Guids { get; set; }
}