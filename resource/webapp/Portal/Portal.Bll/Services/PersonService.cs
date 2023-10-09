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

    public async Task<PersonDto?> TryCreatePersonAsync(PersonAadInfo personAadInfo)
    {
        try
        {
            var person = await GetPersonByIdAsync(personAadInfo.Id);
            if (person != null)
            {
                _logger_.LogInformation($"Person with Id {personAadInfo.Id} already exists. Skipping creation.");
                return null;
            }
            _logger_.LogInformation($"Person with Id {personAadInfo.Id} not found. Creating...");
            
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
            return await _personProvider_.CreatePersonAsync(personDb);

        }
        catch (Exception e)
        {
            _logger_.LogError(e, $"Exception on {nameof(TryCreatePersonAsync)}");
            return null;
        }
    }
}