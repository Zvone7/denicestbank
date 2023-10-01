namespace denicestbankportal.Models;

public class Person
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public int RoleId { get; set; }
    public string Ssn { get; set; }
}