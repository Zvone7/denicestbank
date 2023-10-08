using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portal.Bll.Services;

namespace Portal.Api.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = OpenIdConnectDefaults.AuthenticationScheme)]
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