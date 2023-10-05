using denicestbankportal.Logic;
using denicestbankportal.Models;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace denicestbankportal.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = OpenIdConnectDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class LoanController : Controller
    {
        private readonly LoanService _loanService_;
        private readonly ILogger<LoanController> _logger_;

        public LoanController(LoanService loanService, ILogger<LoanController> logger)
        {
            _loanService_ = loanService ?? throw new ArgumentNullException(nameof(loanService));
            _logger_ = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LoanOverview>>> GetAllLoansOverview()
        {
            var loans = await _loanService_.GetAllLoansOverviewAsync();
            return Ok(loans);
        }

        [HttpGet]
        [Route(nameof(GetAllLoansByPersonId))]
        public async Task<ActionResult<IEnumerable<LoanOverview>>> GetAllLoansByPersonId()
        {
            var aadId = ControllerHelper.ExtractAadId(User);
            var loans = await _loanService_.GetAllLoansByPersonIdAsync(aadId);
            return Ok(loans);
        }


        [HttpPut("{id}")]
        [Route(nameof(ApproveLoan))]
        public async Task<IActionResult> ApproveLoan(Guid loanId)
        {
            var approveResult = await _loanService_.ApproveLoanAsync(loanId);

            return Ok(approveResult);
        }


        [HttpPost]
        public async Task<ActionResult<Loan>> ApplyForLoan(LoanBm loanBm)
        {
            var aadId = ControllerHelper.ExtractAadId(User);
            var createdLoan = await _loanService_.ApplyForLoanAsync(new LoanApplyObj() { Loan = loanBm, Guids = new List<Guid>() { aadId } });
            return CreatedAtAction(nameof(ApplyForLoan), new { id = createdLoan.Id }, createdLoan);
        }
    }
}