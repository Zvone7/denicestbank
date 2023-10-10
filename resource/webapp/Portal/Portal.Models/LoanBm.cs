namespace Portal.Models;

public class LoanBm
{
    public Decimal LoanBaseAmount { get; set; }
    public Decimal Interest { get; set; }
    public String Purpose { get; set; }
}

public class LoanBmAlt : LoanBm
{
    public IEnumerable<Guid> Persons { get; set; }
}