namespace Portal.Models;

public class LoanWithPersons
{
    public LoanDto LoanDto { get; set; }
    public List<PersonDto> Persons { get; set; }
}