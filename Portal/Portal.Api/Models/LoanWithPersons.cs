namespace Portal.Api.Models;

public class LoanWithPersons
{
    public Loan Loan { get; set; }
    public List<Person> Persons { get; set; }
}

public class LoanOverview
{
    public LoanOverview(LoanWithPersons lwp, decimal totalReturned)
    {
        Loan = lwp.Loan;
        Persons = lwp.Persons;
        LoanTotalReturned = totalReturned;
    }
    public Loan Loan { get; set; }
    public List<Person> Persons { get; set; }
    public decimal LoanTotalReturned { get; set; }
}