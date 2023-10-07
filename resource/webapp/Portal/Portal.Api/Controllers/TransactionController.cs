using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portal.Api.Logic;
using Portal.Api.Models;

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
    public async Task<ActionResult<IEnumerable<Transact>>> GenerateTransactions()
    {
        var transactionResults = (await _transactionService.GenerateTransactionsAsync());
        return Ok(transactionResults);
    }

}