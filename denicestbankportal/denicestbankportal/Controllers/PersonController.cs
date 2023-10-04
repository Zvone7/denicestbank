using denicestbankportal.Logic;
using denicestbankportal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace denicestbankportal.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class PersonController : Controller
{
    private readonly PersonService _personService_;

    public PersonController(PersonService personService)
    {
        _personService_ = personService ?? throw new ArgumentNullException(nameof(personService));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Person>>> GetAllPersons()
    {
        var persons = await _personService_.GetAllPersonsAsync();
        return Ok(persons);
    }

    [HttpGet("{loanId}")]
    public async Task<ActionResult<Person>> GetPerson(Guid loanId)
    {
        var person = await _personService_.GetPersonByIdAsync(loanId);
        if (person == null)
        {
            return NotFound();
        }
        return Ok(person);
    }

    // [HttpPut("{id}")]
    // public async Task<IActionResult> UpdatePerson(Person person)
    // {
    //     person.Id = Guid.Parse(RouteData.Values["id"].ToString());
    //     var updatedPerson = await _personService.UpdatePersonAsync(person);
    //
    //     return Ok(updatedPerson);
    // }

    // [HttpDelete("{id}")]
    // public async Task<IActionResult> DeletePerson(Guid id)
    // {
    //     await _personService.DeletePersonAsync(id);
    //     return NoContent();
    // }
}