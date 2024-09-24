using Microsoft.AspNetCore.Mvc;
using TestToday.Models;
using TestToday.Services;
using TestToday.Entities;
using TestToday.Filter;
using TestToday.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;

namespace TestToday.Controllers
{
    /// <summary>
    /// Controller responsible for managing investigation related operations.
    /// </summary>
    /// <remarks>
    /// This Controller provides endpoints for adding, retrieving, updating, and deleting investigation information.
    /// </remarks>
    [Route("api/investigation")]
    [Authorize]
    public class InvestigationController : ControllerBase
    {
        private readonly IInvestigationService _investigationService;

        /// <summary>
        /// Initializes a new instance of the InvestigationController class with the specified context.
        /// </summary>
        /// <param name="iinvestigationservice">The iinvestigationservice to be used by the controller.</param>
        public InvestigationController(IInvestigationService iinvestigationservice)
        {
            _investigationService = iinvestigationservice;
        }

        /// <summary>Adds a new investigation</summary>
        /// <param name="model">The investigation data to be added</param>
        /// <returns>The result of the operation</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [UserAuthorize("Investigation",Entitlements.Create)]
        public IActionResult Post([FromBody] Investigation model)
        {
            var id = _investigationService.Create(model);
            return Ok(new { id });
        }

        /// <summary>Retrieves a list of investigations based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of investigations</returns>
        [HttpGet]
        [UserAuthorize("Investigation",Entitlements.Read)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public IActionResult Get([FromQuery] string filters, string searchTerm, int pageNumber = 1, int pageSize = 10, string sortField = null, string sortOrder = "asc")
        {
            List<FilterCriteria> filterCriteria = null;
            if (pageSize < 1)
            {
                return BadRequest("Page size invalid.");
            }

            if (pageNumber < 1)
            {
                return BadRequest("Page mumber invalid.");
            }

            if (!string.IsNullOrEmpty(filters))
            {
                filterCriteria = JsonHelper.Deserialize<List<FilterCriteria>>(filters);
            }

            var result = _investigationService.Get(filterCriteria, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return Ok(result);
        }

        /// <summary>Retrieves a specific investigation by its primary key</summary>
        /// <param name="id">The primary key of the investigation</param>
        /// <returns>The investigation data</returns>
        [HttpGet]
        [Route("{id:Guid}")]
        [UserAuthorize("Investigation",Entitlements.Read)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public IActionResult GetById([FromRoute] Guid id)
        {
            var result = _investigationService.GetById(id);
            return Ok(result);
        }

        /// <summary>Deletes a specific investigation by its primary key</summary>
        /// <param name="id">The primary key of the investigation</param>
        /// <returns>The result of the operation</returns>
        [HttpDelete]
        [UserAuthorize("Investigation",Entitlements.Delete)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [Route("{id:Guid}")]
        public IActionResult DeleteById([FromRoute] Guid id)
        {
            var status = _investigationService.Delete(id);
            return Ok(new { status });
        }

        /// <summary>Updates a specific investigation by its primary key</summary>
        /// <param name="id">The primary key of the investigation</param>
        /// <param name="updatedEntity">The investigation data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPut]
        [UserAuthorize("Investigation",Entitlements.Update)]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public IActionResult UpdateById(Guid id, [FromBody] Investigation updatedEntity)
        {
            if (id != updatedEntity.Id)
            {
                return BadRequest("Mismatched Id");
            }

            var status = _investigationService.Update(id, updatedEntity);
            return Ok(new { status });
        }

        /// <summary>Updates a specific investigation by its primary key</summary>
        /// <param name="id">The primary key of the investigation</param>
        /// <param name="updatedEntity">The investigation data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPatch]
        [UserAuthorize("Investigation",Entitlements.Update)]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public IActionResult UpdateById(Guid id, [FromBody] JsonPatchDocument<Investigation> updatedEntity)
        {
            if (updatedEntity == null)
                return BadRequest("Patch document is missing.");
            var status = _investigationService.Patch(id, updatedEntity);
            return Ok(new { status });
        }
    }
}