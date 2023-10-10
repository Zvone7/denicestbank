using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portal.Core.Services;
using Portal.Models;

namespace Portal.Api.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/[controller]")]
public class TransactionController : BaseController
{
    private readonly ITransactionService _transactionService_;
    private readonly ILogger<TransactionController> _logger_;

    public TransactionController(
        ITransactionService transactionService,
        ILogger<TransactionController> logger
    ) : base(logger)
    {
        _transactionService_ = transactionService;
        _logger_ = logger;
    }


    [HttpPost]
    public async Task<IActionResult> GenerateTransaction(TransactionGenerationObj transactionGenerationObj)
    {
        var executorId = ExtractAadId(User);
        var transactionResults = (await _transactionService_.GenerateTransactionAsync(
            transactionGenerationObj.PersonId,
            transactionGenerationObj.LoanId,
            executorId));
        return HandleResult(transactionResults);
    }

    [HttpPost]
    [Route(nameof(GenerateTransactions))]
    public async Task<IActionResult> GenerateTransactions()
    {
        var transactionResults = (await _transactionService_.GenerateTransactionsAsync());
        return HandleResult(transactionResults);
    }
}