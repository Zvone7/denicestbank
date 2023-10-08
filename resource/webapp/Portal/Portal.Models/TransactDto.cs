namespace Portal.Models;

public class TransactDto
{
    public Guid Id { get; set; }
    public Guid PersonId { get; set; }
    public Guid LoanId { get; set; }
    public DateTime UpdateDatetimeUtc { get; set; }
    public decimal Amount { get; set; }
}