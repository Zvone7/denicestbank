namespace Portal.Api.Models;

public class LoanLatestState
{
    public Guid LoanId { get; set; }
    public decimal TotalTransacted { get; set; }
}