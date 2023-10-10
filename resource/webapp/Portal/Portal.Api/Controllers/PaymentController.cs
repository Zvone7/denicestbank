using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portal.Core.Services;

namespace Portal.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : BaseController
{
    private readonly IPersonService _personService_;
    private readonly ITransactionService _transactionService_;
    private readonly ILogger<PaymentController> _logger_;

    public PaymentController(
        IPersonService personService,
        ITransactionService transactionService,
        ILogger<PaymentController> logger
    ) : base(logger)
    {
        _personService_ = personService;
        _transactionService_ = transactionService;
        _logger_ = logger;
    }

    [Authorize(AuthenticationSchemes = OpenIdConnectDefaults.AuthenticationScheme)]
    public async Task<IActionResult> Index()
    {
        await TryCreatePersonFromAadUser(_personService_);
        return View();
    }

    [HttpGet]
    [Route("payments")]
    [Authorize(AuthenticationSchemes = OpenIdConnectDefaults.AuthenticationScheme)]
    public async Task<IActionResult> GetPayments(Int32 pageIndex = 1, Int32 pageSize = 10)
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