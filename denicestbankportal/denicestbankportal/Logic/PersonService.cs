using denicestbankportal.Database;
using denicestbankportal.Models;

namespace denicestbankportal.Logic;

public class PersonService
{
    private readonly PersonProvider _personProvider_;

    public PersonService(PersonProvider personProvider)
    {
        _personProvider_ = personProvider ?? throw new ArgumentNullException(nameof(personProvider));
    }

    public async Task<IEnumerable<Person>> GetAllPersonsAsync()
    {
        return await _personProvider_.GetAllPersonsAsync();
    }

    public async Task<Person> GetPersonByIdAsync(Guid id)
    {
        return await _personProvider_.GetPersonByIdAsync(id);
    }

    public async Task<Person> CreatePersonAsync(Person person)
    {
        return await _personProvider_.CreatePersonAsync(person);
    }

    public async Task<Person> UpdatePersonAsync(Person person)
    {
        return await _personProvider_.UpdatePersonAsync(person);
    }

    public async Task DeletePersonAsync(Guid id)
    {
        await _personProvider_.DeletePersonAsync(id);
    }
}