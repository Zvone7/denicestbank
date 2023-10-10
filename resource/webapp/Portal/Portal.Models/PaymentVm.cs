namespace Portal.Models;

public class PaymentVm
{
    public Guid Id { get; set; }
    public Guid PersonId { get; set; }
    public String PersonFullName { get; set; }
    public Guid LoanId { get; set; }
    public String LoanPurpose { get; set; }
    public DateTime UpdateDatetimeUtc { get; set; }
    public Decimal Amount { get; set; }
    
}