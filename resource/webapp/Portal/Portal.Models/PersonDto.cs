namespace Portal.Models;

public class PersonDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public string Ssn { get; set; }
}