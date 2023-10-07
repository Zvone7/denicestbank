namespace Portal.Api.Models;

public class Payment
{
    public Guid Id { get; set; }
    public Guid PersonId { get; set; }
    public String PersonFullName { get; set; }
    public Guid LoanId { get; set; }
    public String LoanPurpose { get; set; }
    public DateTime UpdateDatetimeUtc { get; set; }
    public decimal Amount { get; set; }
    
}