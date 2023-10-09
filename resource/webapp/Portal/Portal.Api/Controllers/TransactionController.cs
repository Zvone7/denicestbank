using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portal.Core.Providers;
using Portal.Core.Services;
using Portal.Models;

namespace Portal.Api.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/[controller]")]
public class TransactionController : BaseController
{
    private readonly ITransactionService _transactionService;
    private readonly ILogger _logger_;

    public TransactionController(
        ITransactionService transactionService,
        ILogger logger
    ) : base(logger)
    {
        _transactionService = transactionService;
        _logger_ = logger;
    }

    [HttpPost]
    [Route(nameof(GenerateTransactions))]
    public async Task<IActionResult> GenerateTransactions()
    {
        var transactionResults = (await _transactionService.GenerateTransactionsAsync());
        return HandleResult(transactionResults);
    }

}