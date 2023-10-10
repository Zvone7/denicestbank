using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portal.Core.Services;
using Portal.Models;

namespace Portal.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonController : BaseController
    {
        private readonly IPersonService _personService_;
        private readonly ILogger<LoanController> _logger_;

        public PersonController(
            IPersonService personService,
            ILogger<LoanController> logger
        ) : base(logger)
        {
            _personService_ = personService;
            _logger_ = logger;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> CreateTestUser()
        {
            var testUserAadInfo = new PersonAadInfo()
            {
                Id = Guid.NewGuid(),
                Email = "test@email.com",
                FullName = "Test"
            };
            var testUser = await _personService_.TryCreatePersonAsync(testUserAadInfo);
            return HandleResult(testUser);
        }
    }
}