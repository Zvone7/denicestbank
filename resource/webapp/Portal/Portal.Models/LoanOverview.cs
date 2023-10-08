namespace Portal.Models;

public class LoanOverview
{
    public LoanOverview(LoanWithPersons lwp, decimal totalReturned)
    {
        LoanDto = lwp.LoanDto;
        Persons = lwp.Persons;
        LoanTotalReturned = totalReturned;
    }
    public LoanDto LoanDto { get; set; }
    public List<PersonDto> Persons { get; set; }
    public decimal LoanTotalReturned { get; set; }
}