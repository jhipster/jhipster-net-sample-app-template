using System.Collections.Generic;
using System.Threading.Tasks;
using JHipsterNet.Pagination;
using JHipsterNet.Pagination.Extensions;
using JHipsterNetSampleApplication.Data;
using JHipsterNetSampleApplication.Data.Extensions;
using JHipsterNetSampleApplication.Models;
using JHipsterNetSampleApplication.Web.Extensions;
using JHipsterNetSampleApplication.Web.Filters;
using JHipsterNetSampleApplication.Web.Rest.Problems;
using JHipsterNetSampleApplication.Web.Rest.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JHipsterNetSampleApplication.Controllers {
    [Authorize]
    [Route("api")]
    [ApiController]
    public class BankAccountController : ControllerBase {
        private const string EntityName = "bankAccount";

        private readonly ApplicationDatabaseContext _applicationDatabaseContext;

        private readonly ILogger<BankAccountController> _log;

        public BankAccountController(ILogger<BankAccountController> log,
            ApplicationDatabaseContext applicationDatabaseContext)
        {
            _log = log;
            _applicationDatabaseContext = applicationDatabaseContext;
        }

        [HttpPost("bank-accounts")]
        [ValidateModel]
        public async Task<ActionResult<BankAccount>> CreateBankAccount([FromBody] BankAccount bankAccount)
        {
            _log.LogDebug($"REST request to save BankAccount : {bankAccount}");
            if (bankAccount.Id != 0)
                throw new BadRequestAlertException("A new bankAccount cannot already have an ID", EntityName,
                    "idexists");

            _applicationDatabaseContext.AddGraph(bankAccount);
            await _applicationDatabaseContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetBankAccount), new { id = bankAccount.Id }, bankAccount)
                    .WithHeaders(HeaderUtil.CreateEntityCreationAlert(EntityName, bankAccount.Id.ToString()));
        }

        [HttpPut("bank-accounts")]
        [ValidateModel]
        public async Task<IActionResult> UpdateBankAccount([FromBody] BankAccount bankAccount)
        {
            _log.LogDebug($"REST request to update BankAccount : {bankAccount}");
            if (bankAccount.Id == 0) throw new BadRequestAlertException("Invalid Id", EntityName, "idnull");
            //TODO catch //DbUpdateConcurrencyException into problem
            _applicationDatabaseContext.Update(bankAccount);
            /* Force the reference navigation property to be in "modified" state.
               This allows to modify it with a null value (the field is nullable).
               This takes into consideration the case of removing the association between the two instances. */
            _applicationDatabaseContext.Entry(bankAccount).Reference(bA => bA.User).IsModified = true;
            await _applicationDatabaseContext.SaveChangesAsync();
            return Ok(bankAccount)
                .WithHeaders(HeaderUtil.CreateEntityUpdateAlert(EntityName, bankAccount.Id.ToString()));
        }

        [HttpGet("bank-accounts")]
        public ActionResult<IEnumerable<BankAccount>> GetAllBankAccounts(IPageable pageable)
        {
            _log.LogDebug("REST request to get a page of BankAccounts");
            var page = _applicationDatabaseContext.BankAccounts
                .Include(bankAccount => bankAccount.User)
                .UsePageable(pageable);
            var headers = PaginationUtil.GeneratePaginationHttpHeaders(page, HttpContext.Request);
            return Ok(page.Content).WithHeaders(headers);
        }

        [HttpGet("bank-accounts/{id}")]
        public async Task<IActionResult> GetBankAccount([FromRoute] long id)
        {
            _log.LogDebug($"REST request to get BankAccount : {id}");
            var result = await _applicationDatabaseContext.BankAccounts
                .Include(bankAccount => bankAccount.User)
                .SingleOrDefaultAsync(bankAccount => bankAccount.Id == id);
            return ActionResultUtil.WrapOrNotFound(result);
        }

        [HttpDelete("bank-accounts/{id}")]
        public async Task<IActionResult> DeleteBankAccount([FromRoute] long id)
        {
            _log.LogDebug($"REST request to delete BankAccount : {id}");
            _applicationDatabaseContext.BankAccounts.RemoveById(id);
            await _applicationDatabaseContext.SaveChangesAsync();
            return Ok().WithHeaders(HeaderUtil.CreateEntityDeletionAlert(EntityName, id.ToString()));
        }
    }
}
