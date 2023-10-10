using LanguageExt.Common;
using Portal.Models;

namespace Portal.Core.Services;

public interface IPersonService
{
    Task<Result<PersonDto>> GetPersonByIdAsync(Guid personId);
    Task<Result<PersonDto>> TryCreatePersonAsync(PersonAadInfo personAadInfo);
}