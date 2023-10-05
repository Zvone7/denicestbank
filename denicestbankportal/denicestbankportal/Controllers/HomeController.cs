using System.Diagnostics;
using denicestbankportal.Logic;
using Microsoft.AspNetCore.Mvc;
using denicestbankportal.Models;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;

namespace denicestbankportal.Controllers;

// [Authorize(AuthenticationSchemes = OpenIdConnectDefaults.AuthenticationScheme)]
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
        // await ControllerHelper.TryCreatePersonFromAadUser(_personService_, User);
        return View();
    }

    public ActionResult<String> Test()
    {
        return "is just a test bro";
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}