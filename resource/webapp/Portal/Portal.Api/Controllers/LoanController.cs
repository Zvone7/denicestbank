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
    public class LoanController : BaseController
    {
        private readonly ILoanService _loanService_;
        private readonly ILogger<LoanController> _logger_;

        public LoanController(
            ILoanService loanService,
            ILogger<LoanController> logger
        ) : base(logger)
        {
            _loanService_ = loanService ?? throw new ArgumentNullException(nameof(loanService));
            _logger_ = logger;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = OpenIdConnectDefaults.AuthenticationScheme + "," + JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAllLoansOverview()
        {
            var loans = await _loanService_.GetAllLoansOverviewAsync();
            return HandleResult(loans);
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = OpenIdConnectDefaults.AuthenticationScheme)]
        [Route(nameof(GetAllLoansByPersonId))]
        public async Task<IActionResult> GetAllLoansByPersonId()
        {
            var aadId = ExtractAadId(User);
            var loans = await _loanService_.GetAllLoansByPersonIdAsync(aadId);
            return HandleResult(loans);
        }

        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = OpenIdConnectDefaults.AuthenticationScheme + "," + JwtBearerDefaults.AuthenticationScheme)]
        [Route(nameof(ApproveLoan))]
        public async Task<IActionResult> ApproveLoan(Guid loanId)
        {
            var aadId = ExtractAadId(User);
            return HandleResult(await _loanService_.ApproveLoanAsync(loanId, aadId));
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = OpenIdConnectDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ApplyForLoan(LoanBm loanBm)
        {
            var aadId = ExtractAadId(User);
            var createdLoan = await _loanService_.ApplyForLoanAsync(new LoanApplication() { Loan = loanBm, Guids = new List<Guid>() { aadId } });
            return HandleResult(createdLoan);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route($"{nameof(ApplyForLoanAlt)}")]
        public async Task<IActionResult> ApplyForLoanAlt(LoanBmAlt loanBm)
        {
            var createdLoan = await _loanService_.ApplyForLoanAsync(new LoanApplication() { Loan = loanBm, Guids = loanBm.Persons });
            return HandleResult(createdLoan);
        }
    }
}