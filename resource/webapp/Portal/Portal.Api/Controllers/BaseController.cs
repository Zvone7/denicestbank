using System.Security.Claims;
using LanguageExt.Common;
using Microsoft.AspNetCore.Mvc;
using Portal.Core.Services;
using Portal.Models;

namespace Portal.Api.Controllers;

public class BaseController : Controller
{
    private readonly ILogger<BaseController> _logger_;
    public BaseController(ILogger<BaseController> logger)
    {
        _logger_ = logger;
    }

    protected async Task<Result<PersonDto>> TryCreatePersonFromAadUser(
        IPersonService personService)
    {
        try
        {
            var aadId = ExtractAadId(User);
            var email = User.Claims.Where(c => c.Type.Contains("identity/claims/name")).Select(c => c.Value).First();
            var personAadInfo = new PersonAadInfo()
            {
                Id = aadId,
                Email = email,
                FullName = User.Claims.Where(c => c.Type == "name").Select(c => c.Value).First()
            };
            return await personService.TryCreatePersonAsync(personAadInfo);
        }
        catch (Exception e)
        {
            _logger_.LogError(e, $"Exception on {nameof(TryCreatePersonFromAadUser)}");
            return new Result<PersonDto>(e);
        }
    }

    protected static Guid ExtractAadId(ClaimsPrincipal user)
    {
        var appIdClaim = user.Claims
            .FirstOrDefault(c => c.Type.Contains("appid"));
        if (appIdClaim != null) return new Guid(appIdClaim.Value);
        
        var aadIdClaim = user.Claims
            .FirstOrDefault(c => c.Type.Contains("identity/claims/objectidentifier"));
        if (aadIdClaim != null) return new Guid(aadIdClaim.Value);
        
        
        throw new Exception("Invalid authentication");
    }

    protected IActionResult HandleResult<T>(Result<T> result)
    {
        var resultHandled = result.Match<IActionResult>(
            succ => Ok(succ),
            exception => BadRequest(new ExceptionDetails(exception)));
        return resultHandled;
    }

    private class ExceptionDetails
    {
        public String Message { get; set; }
        public String? StackTrace { get; set; }
        public ExceptionDetails(Exception e)
        {
            Message = e.Message;
            StackTrace = e.StackTrace;
        }
    }
}