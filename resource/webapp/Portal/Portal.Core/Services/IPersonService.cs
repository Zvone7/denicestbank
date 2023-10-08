using Portal.Models;

namespace Portal.Core.Services;

public interface IPersonService
{
    Task<PersonDto?> GetPersonByIdAsync(Guid personId);
    Task TryCreatePerson(PersonAadInfo personAadInfo);
}