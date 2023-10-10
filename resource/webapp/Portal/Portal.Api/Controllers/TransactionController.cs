using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portal.Core.Services;

namespace Portal.Api.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/[controller]")]
public class TransactionController : BaseController
{
    private readonly ITransactionService _transactionService;
    private readonly ILogger<TransactionController> _logger_;

    public TransactionController(
        ITransactionService transactionService,
        ILogger<TransactionController> logger
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