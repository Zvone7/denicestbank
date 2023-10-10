using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portal.Core.Providers;

namespace Portal.Api.Controllers;

[Route("api/[controller]")]
public class TestController : BaseController
{
    private readonly ILoanProvider _loanProvider_;
    private readonly ILogger<TestController> _logger_;
    public TestController(
        ILoanProvider loanProvider,
        ILogger<TestController> logger
    ) : base(logger)
    {
        _loanProvider_ = loanProvider;
        _logger_ = logger;
    }

    [HttpGet]
    [Route("public")]
    public ActionResult<String> Test()
    {
        _logger_.LogInformation("public test endpoint called");
        return "Unauthenticated works";
    }


    [HttpGet]
    [Authorize(AuthenticationSchemes = OpenIdConnectDefaults.AuthenticationScheme)]
    [Route("user_auth")]
    public ActionResult<String> Test2()
    {
        return "User auth works";
    }


    [HttpGet]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("app_auth")]
    public ActionResult<String> Test3()
    {
        return Ok("App auth works");
    }

    [HttpGet]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("app_auth2")]
    public async Task<IActionResult> Test4()
    {
        // GetAllLoansOverviewAsync
        return HandleResult(await _loanProvider_.GetAllLoansWithPersonsAsync());
    }
}