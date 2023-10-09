using LanguageExt.Common;
using Portal.Models;

namespace Portal.Core.Providers;

public interface IPersonProvider
{
    Task<Result<PersonDto>> GetPersonByIdAsync(Guid personId);
    Task<Result<PersonDto>> CreatePersonAsync(PersonDto personDto);
}