using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portal.Core.Services;
using Portal.Models;

namespace Portal.Api.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/[controller]")]
public class TransactionController : Controller
{
    private readonly ITransactionService _transactionService;

    public TransactionController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpPost]
    [Route(nameof(GenerateTransactions))]
    public async Task<ActionResult<IEnumerable<TransactDto>>> GenerateTransactions()
    {
        var transactionResults = (await _transactionService.GenerateTransactionsAsync());
        return Ok(transactionResults);
    }

}