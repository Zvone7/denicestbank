using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portal.Core.Services;
using Portal.Models;

namespace Portal.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentBaseController : BaseController
{
    private readonly IPersonService _personService_;
    private readonly ITransactionService _transactionService_;

    public PaymentBaseController(
        IPersonService personService,
        ITransactionService transactionService,
        ILogger logger
    ) : base(logger)
    {
        _personService_ = personService;
        _transactionService_ = transactionService;
    }

    [Authorize(AuthenticationSchemes = OpenIdConnectDefaults.AuthenticationScheme)]
    public async Task<IActionResult> Index()
    {
        await TryCreatePersonFromAadUser(_personService_, User);
        return View();
    }

    [HttpGet]
    [Route("payments")]
    [Authorize(AuthenticationSchemes = OpenIdConnectDefaults.AuthenticationScheme)]
    public async Task<IActionResult> GetPayments(int pageIndex = 1, int pageSize = 10)
    {
        return HandleResult(await _transactionService_.GetLatestPaymentsAsync(pageIndex, pageSize));
    }


    [HttpPost]
    [Route("generate")]
    [Authorize(AuthenticationSchemes = OpenIdConnectDefaults.AuthenticationScheme)]
    public async Task<IActionResult> GeneratePayments()
    {
        return HandleResult(await _transactionService_.GenerateTransactionsAsync());
    }
}