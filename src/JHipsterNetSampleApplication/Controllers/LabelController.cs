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
    public class LabelController : ControllerBase {
        private const string EntityName = "label";

        private readonly ApplicationDatabaseContext _applicationDatabaseContext;

        private readonly ILogger<LabelController> _log;

        public LabelController(ILogger<LabelController> log,
            ApplicationDatabaseContext applicationDatabaseContext)
        {
            _log = log;
            _applicationDatabaseContext = applicationDatabaseContext;
        }

        [HttpPost("labels")]
        [ValidateModel]
        public async Task<ActionResult<Label>> CreateLabel([FromBody] Label label)
        {
            _log.LogDebug($"REST request to save Label : {label}");
            if (label.Id != 0)
                throw new BadRequestAlertException("A new label cannot already have an ID", EntityName,
                    "idexists");

            _applicationDatabaseContext.Labels.Add(label);
            await _applicationDatabaseContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetLabel), new { id = label.Id }, label)
                .WithHeaders(HeaderUtil.CreateEntityCreationAlert(EntityName, label.Id.ToString()));
        }

        [HttpPut("labels")]
        [ValidateModel]
        public async Task<IActionResult> UpdateLabel([FromBody] Label label)
        {
            _log.LogDebug($"REST request to update Label : {label}");
            if (label.Id == 0) throw new BadRequestAlertException("Invalid Id", EntityName, "idnull");
            //TODO catch //DbUpdateConcurrencyException into problem
            _applicationDatabaseContext.Entry(label).State = EntityState.Modified;
            await _applicationDatabaseContext.SaveChangesAsync();
            return Ok(label)
                .WithHeaders(HeaderUtil.CreateEntityUpdateAlert(EntityName, label.Id.ToString()));
        }

        [HttpGet("labels")]
        public ActionResult<IEnumerable<Label>> GetAllLabels(IPageable pageable)
        {
            _log.LogDebug("REST request to get a page of Labels");
            var page = _applicationDatabaseContext.Labels.UsePageable(pageable);
            var headers = PaginationUtil.GeneratePaginationHttpHeaders(page, HttpContext.Request);
            return Ok(page.Content).WithHeaders(headers);
        }

        [HttpGet("labels/{id}")]
        public async Task<IActionResult> GetLabel([FromRoute] long id)
        {
            _log.LogDebug($"REST request to get Label : {id}");
            var result =
                await _applicationDatabaseContext.Labels.SingleOrDefaultAsync(label =>
                    label.Id == id);
            return ActionResultUtil.WrapOrNotFound(result);
        }

        [HttpDelete("labels/{id}")]
        public async Task<IActionResult> DeleteLabel([FromRoute] long id)
        {
            _log.LogDebug($"REST request to delete Label : {id}");
            _applicationDatabaseContext.Labels.RemoveById(id);
            await _applicationDatabaseContext.SaveChangesAsync();
            return Ok().WithHeaders(HeaderUtil.CreateEntityDeletionAlert(EntityName, id.ToString()));
        }
    }
}
