using System.Diagnostics;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portal.Api.Models;
using Portal.Bll.Services;

namespace Portal.Api.Controllers;

[Authorize(AuthenticationSchemes = OpenIdConnectDefaults.AuthenticationScheme)]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger_;
    private readonly PersonService _personService_;

    public HomeController(ILogger<HomeController> logger, PersonService personService)
    {
        _logger_ = logger;
        _personService_ = personService;
    }

    public async Task<IActionResult> Index()
    {
        await ControllerHelper.TryCreatePersonFromAadUser(_personService_, User);
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}