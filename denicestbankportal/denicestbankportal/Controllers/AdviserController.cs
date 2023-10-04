using denicestbankportal.Logic;
using denicestbankportal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace denicestbankportal.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class AdviserController : Controller
{
    private readonly PersonService _personService_;

    public AdviserController(PersonService personService)
    {
        _personService_ = personService;
    }

    public async Task<IActionResult> Index()
    {
        await ControllerHelper.TryCreatePersonFromAadUser(_personService_, User);
        return View();
    }
}