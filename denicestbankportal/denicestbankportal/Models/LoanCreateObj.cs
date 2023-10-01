namespace denicestbankportal.Models;

public class LoanCreateObj
{
    public Loan Loan { get; set; }
    public IEnumerable<Guid> Guids { get; set; }
}