using Microsoft.Extensions.Logging;
using Portal.Core.Generation;
using Portal.Core.Providers;
using Portal.Core.Services;
using Portal.Models;

namespace Portal.Bll.Services;

public class PersonService : IPersonService
{
    private readonly IPersonProvider _personProvider_;
    private readonly IRandomGenerator _randomGenerator_;
    private readonly ILogger<PersonService> _logger_;

    public PersonService(
        IPersonProvider personProvider,
        IRandomGenerator randomGenerator,
        ILogger<PersonService> logger
    )
    {
        _personProvider_ = personProvider ?? throw new ArgumentNullException(nameof(personProvider));
        _randomGenerator_ = randomGenerator;
        _logger_ = logger;
    }

    public async Task<PersonDto?> GetPersonByIdAsync(Guid personId)
    {
        try
        {
            var person = await _personProvider_.GetPersonByIdAsync(personId);
            if (person == null) throw new KeyNotFoundException($"Person with Id {personId} not found.");
            return person;
        }
        catch (Exception e)
        {
            _logger_.LogError(e, $"Exception on {nameof(GetPersonByIdAsync)}");
            return null;
        }
    }

    public async Task TryCreatePerson(PersonAadInfo personAadInfo)
    {
        try
        {
            var person = await GetPersonByIdAsync(personAadInfo.Id);
            if (person != null)
            {
                _logger_.LogInformation($"Person with Id {personAadInfo.Id} already exists. Skipping creation.");
                return;
            }
            _logger_.LogInformation($"Person with Id {personAadInfo.Id} not found. Creating...");

            var assignRole = personAadInfo.Email.ToLowerInvariant() switch
            {
                var email when email.Contains("adviser") => "adviser",
                var email when email.Contains("customer") => "customer",
                _ => "admin"
            };
            var personDb = new PersonDto()
            {
                Id = personAadInfo.Id,
                Email = personAadInfo.Email,
                FullName = personAadInfo.FullName,
                Role = assignRole,
                Ssn = _randomGenerator_.GenerateSsn()

            };
            await _personProvider_.CreatePersonAsync(personDb);

        }
        catch (Exception e)
        {
            _logger_.LogError(e, $"Exception on {nameof(TryCreatePerson)}");
        }
    }
}