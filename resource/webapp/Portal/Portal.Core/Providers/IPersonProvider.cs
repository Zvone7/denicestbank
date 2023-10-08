using Portal.Models;

namespace Portal.Core.Providers;

public interface IPersonProvider
{
    Task<PersonDto> GetPersonByIdAsync(Guid personId);
    Task<PersonDto> CreatePersonAsync(PersonDto personDto);
}