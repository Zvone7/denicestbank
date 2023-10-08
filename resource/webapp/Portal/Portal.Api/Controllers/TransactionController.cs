using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portal.Api.Models;
using Portal.Bll.Services;
using Portal.Models;

namespace Portal.Api.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/[controller]")]
public class TransactionController : Controller
{
    private readonly TransactionService _transactionService;

    public TransactionController(TransactionService transactionService)
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