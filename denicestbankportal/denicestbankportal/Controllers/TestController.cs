using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace denicestbankportal.Controllers;

[Route("api/[controller]")]
public class TestController : Controller
{
    public TestController() {}

    [HttpGet]
    [Route("public")]
    public ActionResult<String> Test()
    {
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
        return "App auth works";
    }
}