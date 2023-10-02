using denicestbankportal.Logic;
using denicestbankportal.Models;
using Microsoft.AspNetCore.Mvc;

namespace denicestbankportal.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomerController : Controller
{

    public CustomerController()
    {
    }

    public IActionResult Index()
    {
        return View();
    }
}