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

namespace JHipsterNetSampleApplication.Web.Rest {
    [Authorize]
    [Route("api")]
    [ApiController]
    public class OperationResource : ControllerBase {
        private const string EntityName = "operation";

        private readonly ApplicationDatabaseContext _applicationDatabaseContext;

        private readonly ILogger<OperationResource> _log;

        public OperationResource(ILogger<OperationResource> log, ApplicationDatabaseContext applicationDatabaseContext)
        {
            _log = log;
            _applicationDatabaseContext = applicationDatabaseContext;
        }

        [HttpPost("operations")]
        [ValidateModel]
        public async Task<ActionResult<Operation>> CreateOperation([FromBody] Operation operation)
        {
            _log.LogDebug($"REST request to save Operation : {operation}");
            if (operation.Id != null)
                throw new BadRequestAlertException("A new operation cannot already have an ID", EntityName, "idexists");

            _applicationDatabaseContext.Operations.Add(operation);
            await _applicationDatabaseContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetOperation), new {id = operation.Id}, operation)
                .WithHeaders(HeaderUtil.CreateEntityCreationAlert(EntityName, operation.Id));
        }

        [HttpPut("operations")]
        [ValidateModel]
        public async Task<IActionResult> UpdateOperation([FromBody] Operation operation)
        {
            _log.LogDebug($"REST request to update Operation : {operation}");
            if (operation.Id == null) throw new BadRequestAlertException("Invalid Id", EntityName, "idnull");
            //TODO catch //DbUpdateConcurrencyException into problem
            _applicationDatabaseContext.Entry(operation).State = EntityState.Modified;
            await _applicationDatabaseContext.SaveChangesAsync();
            return Ok(operation).WithHeaders(HeaderUtil.CreateEntityUpdateAlert(EntityName, operation.Id));
        }

        [HttpGet("operations")]
        public ActionResult<IEnumerable<Operation>> GetAllOperations(IPageable pageable)
        {
            _log.LogDebug("REST request to get a page of Operations");
            var page = _applicationDatabaseContext.Operations.UsePageable(pageable);
            var headers = PaginationUtil.GeneratePaginationHttpHeaders(page, HttpContext.Request);
            return Ok(page.Content).WithHeaders(headers);
        }

        [HttpGet("operations/{id}")]
        public async Task<IActionResult> GetOperation([FromRoute] string id)
        {
            _log.LogDebug($"REST request to get Operation : {id}");
            var result =
                await _applicationDatabaseContext.Operations.SingleOrDefaultAsync(operation => operation.Id == id);
            return ActionResultUtil.WrapOrNotFound(result);
        }

        [HttpDelete("operations/{id}")]
        public async Task<IActionResult> DeleteOperation([FromRoute] string id)
        {
            _log.LogDebug($"REST request to delete Operation : {id}");
            _applicationDatabaseContext.Operations.RemoveById(id);
            await _applicationDatabaseContext.SaveChangesAsync();
            return Ok().WithHeaders(HeaderUtil.CreateEntityDeletionAlert(EntityName, id));
        }
    }
}
