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
    /// Controller responsible for managing productmanufacture related operations.
    /// </summary>
    /// <remarks>
    /// This Controller provides endpoints for adding, retrieving, updating, and deleting productmanufacture information.
    /// </remarks>
    [Route("api/productmanufacture")]
    [Authorize]
    public class ProductManufactureController : ControllerBase
    {
        private readonly IProductManufactureService _productManufactureService;

        /// <summary>
        /// Initializes a new instance of the ProductManufactureController class with the specified context.
        /// </summary>
        /// <param name="iproductmanufactureservice">The iproductmanufactureservice to be used by the controller.</param>
        public ProductManufactureController(IProductManufactureService iproductmanufactureservice)
        {
            _productManufactureService = iproductmanufactureservice;
        }

        /// <summary>Adds a new productmanufacture</summary>
        /// <param name="model">The productmanufacture data to be added</param>
        /// <returns>The result of the operation</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        [UserAuthorize("ProductManufacture",Entitlements.Create)]
        public IActionResult Post([FromBody] ProductManufacture model)
        {
            var id = _productManufactureService.Create(model);
            return Ok(new { id });
        }

        /// <summary>Retrieves a list of productmanufactures based on specified filters</summary>
        /// <param name="filters">The filter criteria in JSON format. Use the following format: [{"PropertyName": "PropertyName", "Operator": "Equal", "Value": "FilterValue"}] </param>
        /// <param name="searchTerm">To searching data.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <param name="sortField">The entity's field name to sort.</param>
        /// <param name="sortOrder">The sort order asc or desc.</param>
        /// <returns>The filtered list of productmanufactures</returns>
        [HttpGet]
        [UserAuthorize("ProductManufacture",Entitlements.Read)]
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

            var result = _productManufactureService.Get(filterCriteria, searchTerm, pageNumber, pageSize, sortField, sortOrder);
            return Ok(result);
        }

        /// <summary>Retrieves a specific productmanufacture by its primary key</summary>
        /// <param name="id">The primary key of the productmanufacture</param>
        /// <returns>The productmanufacture data</returns>
        [HttpGet]
        [Route("{id:Guid}")]
        [UserAuthorize("ProductManufacture",Entitlements.Read)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public IActionResult GetById([FromRoute] Guid id)
        {
            var result = _productManufactureService.GetById(id);
            return Ok(result);
        }

        /// <summary>Deletes a specific productmanufacture by its primary key</summary>
        /// <param name="id">The primary key of the productmanufacture</param>
        /// <returns>The result of the operation</returns>
        [HttpDelete]
        [UserAuthorize("ProductManufacture",Entitlements.Delete)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Produces("application/json")]
        [Route("{id:Guid}")]
        public IActionResult DeleteById([FromRoute] Guid id)
        {
            var status = _productManufactureService.Delete(id);
            return Ok(new { status });
        }

        /// <summary>Updates a specific productmanufacture by its primary key</summary>
        /// <param name="id">The primary key of the productmanufacture</param>
        /// <param name="updatedEntity">The productmanufacture data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPut]
        [UserAuthorize("ProductManufacture",Entitlements.Update)]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public IActionResult UpdateById(Guid id, [FromBody] ProductManufacture updatedEntity)
        {
            if (id != updatedEntity.Id)
            {
                return BadRequest("Mismatched Id");
            }

            var status = _productManufactureService.Update(id, updatedEntity);
            return Ok(new { status });
        }

        /// <summary>Updates a specific productmanufacture by its primary key</summary>
        /// <param name="id">The primary key of the productmanufacture</param>
        /// <param name="updatedEntity">The productmanufacture data to be updated</param>
        /// <returns>The result of the operation</returns>
        [HttpPatch]
        [UserAuthorize("ProductManufacture",Entitlements.Update)]
        [Route("{id:Guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Produces("application/json")]
        public IActionResult UpdateById(Guid id, [FromBody] JsonPatchDocument<ProductManufacture> updatedEntity)
        {
            if (updatedEntity == null)
                return BadRequest("Patch document is missing.");
            var status = _productManufactureService.Patch(id, updatedEntity);
            return Ok(new { status });
        }
    }
}