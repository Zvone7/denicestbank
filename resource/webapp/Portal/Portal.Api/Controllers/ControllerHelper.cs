using System.Security.Claims;
using Portal.Core.Services;
using Portal.Models;

namespace Portal.Api.Controllers;

public static class ControllerHelper
{
    public static async Task TryCreatePersonFromAadUser(
        IPersonService personService, 
        ClaimsPrincipal user)
    {
        try
        {
            var aadId = ExtractAadId(user);
            var email = user.Claims.Where(c => c.Type.Contains("identity/claims/name")).Select(c => c.Value).First();
            var personAadInfo = new PersonAadInfo()
            {
                Id = aadId,
                Email = email,
                FullName = user.Claims.Where(c => c.Type == "name").Select(c => c.Value).First()
            };
            await personService.TryCreatePersonAsync(personAadInfo);
        }
        catch (Exception e)
        {
            Console.WriteLine("Could not create person from AAD identity:", e);
        }
    }

    public static Guid ExtractAadId(ClaimsPrincipal user)
    {
        var aadId = user.Claims.Where(c => c.Type.Contains("identity/claims/objectidentifier")).Select(c => c.Value).First();
        return new Guid(aadId);
    }
}