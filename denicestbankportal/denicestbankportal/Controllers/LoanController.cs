
using denicestbankportal.Database;
using denicestbankportal.Logic;
using denicestbankportal.Models;
using Microsoft.AspNetCore.Mvc;

namespace denicestbankportal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoanController : ControllerBase
    {
        private readonly LoanService _loanService;

        public LoanController(LoanService loanService)
        {
            _loanService = loanService ?? throw new ArgumentNullException(nameof(loanService));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LoanOverview>>> GetAllLoans()
        {
            var loans = await _loanService.GetAllLoansAsync();
            return Ok(loans);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<LoanOverview>> GetLoan(Guid id)
        {
            var loan = await _loanService.GetLoanAsync(id);
            if (loan == null)
            {
                return NotFound();
            }
            return Ok(loan);
        }


        [HttpPost]
        public async Task<ActionResult<Loan>> CreateLoan(LoanCreateObj loanCreateObj)
        {
            var createdLoan = await _loanService.CreateLoanAsync(loanCreateObj.Loan, loanCreateObj.Guids);
            return CreatedAtAction(nameof(GetLoan), new { id = loanCreateObj.Loan.Id }, createdLoan);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLoan(Loan loan)
        {
            loan.Id = Guid.Parse(RouteData.Values["id"].ToString());
            var updatedLoan = await _loanService.UpdateLoanAsync(loan);

            return Ok(updatedLoan);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLoan(Guid id)
        {
            await _loanService.DeleteLoanAsync(id);
            return NoContent();
        }
    }
}