using denicestbankportal.Logic;
using denicestbankportal.Models;
using Microsoft.AspNetCore.Mvc;

namespace denicestbankportal.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdviserController : Controller
{

    public AdviserController()
    {
    }

    public IActionResult Index()
    {
        return View();
    }
}