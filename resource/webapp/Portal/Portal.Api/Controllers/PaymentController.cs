using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portal.Core.Services;
using Portal.Models;

namespace Portal.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : Controller
{
    private readonly IPersonService _personService_;
    private readonly ITransactionService _transactionService_;

    public PaymentController(IPersonService personService, ITransactionService transactionService)
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
    public async Task<IEnumerable<PaymentVm>> GetPayments(int pageIndex = 1, int pageSize = 10)
    {
        return await _transactionService_.GetLatestPaymentsAsync(pageIndex, pageSize);
    }


    [HttpPost]
    [Route("generate")]
    [Authorize(AuthenticationSchemes = OpenIdConnectDefaults.AuthenticationScheme)]
    public async Task<IEnumerable<TransactDto>> GeneratePayments()
    {
        return await _transactionService_.GenerateTransactionsAsync();
    }
}