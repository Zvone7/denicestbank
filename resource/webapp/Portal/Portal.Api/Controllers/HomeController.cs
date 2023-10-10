using System.Diagnostics;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portal.Api.Models;
using Portal.Core.Services;

namespace Portal.Api.Controllers;

[Authorize(AuthenticationSchemes = OpenIdConnectDefaults.AuthenticationScheme)]
public class HomeController : BaseController
{
    private readonly ILogger<HomeController> _logger_;
    private readonly IPersonService _personService_;

    public HomeController(
        IPersonService personService,
        ILogger<HomeController> logger
    ) : base(logger)
    {
        _logger_ = logger;
        _personService_ = personService;
    }

    public async Task<IActionResult> Index()
    {
        await TryCreatePersonFromAadUser(_personService_);
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}