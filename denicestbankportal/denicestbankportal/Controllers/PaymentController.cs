using denicestbankportal.Logic;
using denicestbankportal.Models;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace denicestbankportal.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : Controller
{
    private readonly PersonService _personService_;
    private readonly TransactionService _transactionService_;

    public PaymentController(PersonService personService, TransactionService transactionService)
    {
        _personService_ = personService;
        _transactionService_ = transactionService;
    }

    [Authorize(AuthenticationSchemes = OpenIdConnectDefaults.AuthenticationScheme)]
    public async Task<IActionResult> Index()
    {
        await ControllerHelper.TryCreatePersonFromAadUser(_personService_, User);
        return View();
    }

    [HttpGet]
    [Route("payments")]
    [Authorize(AuthenticationSchemes = OpenIdConnectDefaults.AuthenticationScheme)]
    public async Task<IEnumerable<Payment>> GetPayments(int pageIndex = 1, int pageSize = 10)
    {
        return await _transactionService_.GetLatestPayments(pageIndex, pageSize);
    }


    [HttpPost]
    [Route("generate")]
    [Authorize(AuthenticationSchemes = OpenIdConnectDefaults.AuthenticationScheme)]
    public async Task<IEnumerable<Transact>> GeneratePayments()
    {
        return await _transactionService_.GenerateTransactionsAsync();
    }
}