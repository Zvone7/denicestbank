using System.Security.Claims;
using denicestbankportal.Logic;
using denicestbankportal.Models;

namespace denicestbankportal.Controllers;

public static class ControllerHelper
{
    public static async Task TryCreatePersonFromAadUser(PersonService personService, ClaimsPrincipal user)
    {
        var aadId = ExtractAadId(user);
        var email = user.Claims.Where(c => c.Type.Contains("identity/claims/name")).Select(c => c.Value).First();
        var personAadInfo = new PersonAadInfo()
        {
            Id = aadId,
            Email = email,
            FullName = user.Claims.Where(c => c.Type == "name").Select(c => c.Value).First()
        };
        await personService.CreatePersonIfItDoesntExistAsync(personAadInfo);
    }

    public static Guid ExtractAadId(ClaimsPrincipal user)
    {
        var aadId = user.Claims.Where(c => c.Type.Contains("identity/claims/objectidentifier")).Select(c => c.Value).First();
        return new Guid(aadId);
    }
}