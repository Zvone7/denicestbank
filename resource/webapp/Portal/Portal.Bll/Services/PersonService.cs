using LanguageExt.Common;
using Microsoft.Extensions.Logging;
using Portal.Core.Generation;
using Portal.Core.Providers;
using Portal.Core.Services;
using Portal.Models;
using Portal.Core.Extensions;

namespace Portal.Bll.Services;

public class PersonService : IPersonService
{
    private readonly IPersonProvider _personProvider_;
    private readonly IRandomGenerator _randomGenerator_;
    private readonly ILogger<IPersonService> _logger_;

    public PersonService(
        IPersonProvider personProvider,
        IRandomGenerator randomGenerator,
        ILogger<IPersonService> logger
    )
    {
        _personProvider_ = personProvider ?? throw new ArgumentNullException(nameof(personProvider));
        _randomGenerator_ = randomGenerator;
        _logger_ = logger;
    }

    public async Task<Result<PersonDto>> GetPersonByIdAsync(Guid personId)
    {
        try
        {
            var person = await _personProvider_.GetPersonByIdAsync(personId);
            var personNotNullValidation = person.IsNotNull();
            var personResult = personNotNullValidation.Bind(_ => person);
            return personResult;
        }
        catch (Exception e)
        {
            _logger_.LogError(e, $"Exception on {nameof(GetPersonByIdAsync)}");
            return new Result<PersonDto>(e);
        }
    }

    public async Task<Result<PersonDto>> TryCreatePersonAsync(PersonAadInfo personAadInfo)
    {
        try
        {
            var personExistsResult = await GetPersonByIdAsync(personAadInfo.Id);
            var createdPerson = await personExistsResult.MatchAsync(async person =>
                {
                    _logger_.LogInformation($"Person with Id {personAadInfo.Id} already exists. Skipping creation.");
                    return await Task.FromResult(person);
                },
                _ => CreatePerson(personAadInfo));
            return createdPerson;
        }
        catch (Exception e)
        {
            _logger_.LogError(e, $"Exception on {nameof(TryCreatePersonAsync)}");
            return new Result<PersonDto>(e);
        }
    }
    private async Task<Result<PersonDto>> CreatePerson(PersonAadInfo personAadInfo)
    {
        _logger_.LogInformation($"Creating Person with Id {personAadInfo.Id}.");
        var assignRole = personAadInfo.Email.ToLowerInvariant() switch
        {
            var email when email.Contains("adviser") => PersonRole.adviser,
            var email when email.Contains("customer") => PersonRole.customer,
            _ => PersonRole.admin
        };
        var personDb = new PersonDto()
        {
            Id = personAadInfo.Id,
            Email = personAadInfo.Email,
            FullName = personAadInfo.FullName,
            Role = assignRole.ToString(),
            Ssn = _randomGenerator_.GenerateSsn()

        };
        var createPersonResult = await _personProvider_.CreatePersonAsync(personDb);
        return createPersonResult;
    }
}