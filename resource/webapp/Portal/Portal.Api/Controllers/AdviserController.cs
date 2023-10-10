using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portal.Core.Services;

namespace Portal.Api.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = OpenIdConnectDefaults.AuthenticationScheme)]
[Route("api/[controller]")]
public class AdviserController : BaseController
{
    private readonly IPersonService _personService_;
    private readonly ILogger<AdviserController> _logger_;

    public AdviserController(
        IPersonService personService,
        ILogger<AdviserController> logger
    ) : base(logger)
    {
        _personService_ = personService;
        _logger_ = logger;
    }

    public async Task<IActionResult> Index()
    {
        // await TryCreatePersonFromAadUser(_personService_, User);
        return View();
    }
}