namespace Portal.Models;

public class LoanLogDto
{
    public Int32 Id { get; set; }
    public Guid LoanId { get; set; }
    public Guid CreatedBy { get; set; }
    public String FieldName { get; set; }
    public String NewFieldValue { get; set; }
    public DateTime CreatedDatetimeUtc { get; set; }
}