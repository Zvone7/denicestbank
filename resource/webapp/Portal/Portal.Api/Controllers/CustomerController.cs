using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portal.Core.Services;

namespace Portal.Api.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = OpenIdConnectDefaults.AuthenticationScheme)]
[Route("api/[controller]")]
public class CustomerController : BaseController
{
    private readonly IPersonService _personService_;
    private readonly ILogger<CustomerController> _logger_;

    public CustomerController(
        IPersonService personService,
        ILogger<CustomerController> logger
    ) : base(logger)
    {
        _personService_ = personService;
        _logger_ = logger;
    }

    public async Task<IActionResult> Index()
    {
        await TryCreatePersonFromAadUser(_personService_, User);
        return View();
    }
}