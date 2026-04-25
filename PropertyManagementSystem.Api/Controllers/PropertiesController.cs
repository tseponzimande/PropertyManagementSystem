namespace PropertyManagementSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    [AllowAnonymous]
    public class PropertiesController(IPropertyService propertyService) : ControllerBase
    {
        private readonly IPropertyService _propertyService = propertyService;

        /// <summary>
        /// Retrieves all properties in the system.
        /// </summary>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var properties = await _propertyService.GetAllPropertiesAsync();
            return Ok(properties);
        }

        /// <summary>
        /// Retrieves a property by its unique ID.
        /// </summary>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var property = await _propertyService.GetPropertyByIdAsync(id);
            if (property == null)
                return NotFound(new { message = "Property not found" });

            return Ok(property);
        }

        /// <summary>
        /// Retrieves all properties owned by a specific owner.
        /// </summary>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("owner/{ownerId}")]
        public async Task<IActionResult> GetByOwner(Guid ownerId)
        {
            var properties = await _propertyService.GetPropertiesByOwnerAsync(ownerId);
            return Ok(properties);
        }

        /// <summary>
        /// Retrieves all approved properties.
        /// </summary>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("approved")]
        public async Task<IActionResult> GetApproved()
        {
            var properties = await _propertyService.GetApprovedPropertiesAsync();
            return Ok(properties);
        }

        /// <summary>
        /// Creates a new property.
        /// </summary>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PropertyDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdProperty = await _propertyService.CreatePropertyAsync(dto);

            if (createdProperty == null)
                return BadRequest(new { message = "Property creation failed. Owner not found or invalid data." });

            return CreatedAtAction(nameof(GetById),
                new { id = createdProperty.Id }, createdProperty);
        }

        /// <summary>
        /// Updates an existing property.
        /// </summary>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] PropertyDto dto)
        {
            if (id != dto.Id)
                return BadRequest(new { message = "ID mismatch" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _propertyService.UpdatePropertyAsync(dto);

            if (!result)
                return NotFound(new { message = "Property not found" });

            return NoContent();
        }

        /// <summary>
        /// Deletes a property from the system.
        /// </summary>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _propertyService.DeletePropertyAsync(id);

            if (!result)
                return NotFound(new { message = "Property not found" });

            return NoContent();
        }

        /// <summary>
        /// Approves a property (Admin only).
        /// </summary>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPatch("{id}/approve")]
        public async Task<IActionResult> Approve(Guid id)
        {
            var result = await _propertyService.ApprovePropertyAsync(id);

            if (!result)
                return NotFound(new { message = "Property not found" });

            return Ok(new { message = "Property approved successfully" });
        }

        /// <summary>
        /// Rejects a property (Admin only).
        /// </summary>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPatch("{id}/reject")]
        public async Task<IActionResult> Reject(Guid id)
        {
            var result = await _propertyService.RejectPropertyAsync(id);

            if (!result)
                return NotFound(new { message = "Property not found" });

            return Ok(new { message = "Property rejected successfully" });
        }
    }

}
