using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portal.Core.Services;

namespace Portal.Api.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = OpenIdConnectDefaults.AuthenticationScheme)]
[Route("api/[controller]")]
public class CustomerController : Controller
{
    private readonly IPersonService _personService_;

    public CustomerController(IPersonService personService)
    {
        _personService_ = personService;
    }

    public async Task<IActionResult> Index()
    {
        await ControllerHelper.TryCreatePersonFromAadUser(_personService_, User);
        return View();
    }
}