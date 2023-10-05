using System.Data;
using denicestbankportal.Database;
using denicestbankportal.Models;

namespace denicestbankportal.Logic;

public class PersonService
{
    private readonly PersonProvider _personProvider_;
    private readonly ILogger<PersonService> _logger_;

    public PersonService(PersonProvider personProvider, ILogger<PersonService> logger)
    {
        _personProvider_ = personProvider ?? throw new ArgumentNullException(nameof(personProvider));
        _logger_ = logger;
    }

    public async Task<IEnumerable<Person>> GetAllPersonsAsync()
    {
        return await _personProvider_.GetAllPersonsAsync();
    }

    public async Task<Person?> GetPersonByIdAsync(Guid personId)
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

    public async Task<Person> CreatePersonAsync(Person person)
    {
        return await _personProvider_.CreatePersonAsync(person);
    }

    public async Task CreatePersonIfItDoesntExistAsync(PersonAadInfo personAadInfo)
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
            var personDb = new Person()
            {
                Id = personAadInfo.Id,
                Email = personAadInfo.Email,
                FullName = personAadInfo.FullName,
                Role = assignRole,
                Ssn = GenerateRandomSsn()

            };
            await _personProvider_.CreatePersonAsync(personDb);

        }
        catch (Exception e)
        {
            _logger_.LogError(e, $"Exception on {nameof(CreatePersonIfItDoesntExistAsync)}");
        }
    }

    // public async Task<Person> UpdatePersonAsync(Person person)
    // {
    //     return await _personProvider_.UpdatePersonAsync(person);
    // }

    // public async Task DeletePersonAsync(Guid id)
    // {
    //     await _personProvider_.DeletePersonAsync(id);
    // }
    
    private string GenerateRandomSsn()
    {
        var random = new Random();
        var areaNumber = random.Next(1, 899);
        var groupNumber = random.Next(1, 99);
        var serialNumber = random.Next(1, 9999);
        return $"{areaNumber:D3}-{groupNumber:D2}-{serialNumber:D4}";
    }
}