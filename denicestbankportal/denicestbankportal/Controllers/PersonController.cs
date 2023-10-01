using denicestbankportal.Logic;
using denicestbankportal.Models;
using Microsoft.AspNetCore.Mvc;

namespace denicestbankportal.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PersonController : ControllerBase
{
    private readonly PersonService _personService;

    public PersonController(PersonService personService)
    {
        _personService = personService ?? throw new ArgumentNullException(nameof(personService));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Person>>> GetAllPersons()
    {
        var persons = await _personService.GetAllPersonsAsync();
        return Ok(persons);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Person>> GetPerson(Guid id)
    {
        var person = await _personService.GetPersonByIdAsync(id);
        if (person == null)
        {
            return NotFound();
        }
        return Ok(person);
    }

    [HttpPost]
    public async Task<ActionResult<Person>> CreatePerson(Person person)
    {
        var createdPerson = await _personService.CreatePersonAsync(person);
        return CreatedAtAction(nameof(GetPerson), new { id = person.Id }, createdPerson);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePerson(Person person)
    {
        person.Id = Guid.Parse(RouteData.Values["id"].ToString());
        var updatedPerson = await _personService.UpdatePersonAsync(person);

        return Ok(updatedPerson);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePerson(Guid id)
    {
        await _personService.DeletePersonAsync(id);
        return NoContent();
    }
}