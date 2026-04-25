namespace PropertyManagementSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    //[Authorize]
    [AllowAnonymous]
    public class UnitsController(IUnitService unitService) : ControllerBase
    {
        private readonly IUnitService _unitService = unitService;


        /// <summary>
        /// GetAll
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var units = await _unitService.GetAllUnitsAsync();
            return Ok(units);
        }

        /// <summary>
        /// GetById
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var unit = await _unitService.GetUnitByIdAsync(id);
            if (unit == null)
            {
                return NotFound(new { message = "Unit not found" });
            }
            return Ok(unit);
        }

        /// <summary>
        /// GetByProperty
        /// </summary>
        /// <param name="propertyId"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status304NotModified)]
        [ProducesResponseType(StatusCodes.Status206PartialContent)]
        [HttpGet("property/{propertyId}")]
        public async Task<IActionResult> GetByProperty(Guid propertyId)
        {
            var units = await _unitService.GetUnitsByPropertyAsync(propertyId);
            return Ok(units);
        }

        /// <summary>
        /// GetAvailable
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpGet("available")]
        public async Task<IActionResult> GetAvailable()
        {
            var units = await _unitService.GetAvailableUnitsAsync();
            return Ok(units);
        }

        /// <summary>
        /// Create
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UnitDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var createdUnit = await _unitService.CreateUnitAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = createdUnit?.Id }, createdUnit);
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UnitDto dto)
        {
            if (id != dto.Id)
            {
                return BadRequest(new { message = "ID mismatch" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _unitService.UpdateUnitAsync(dto);

            if (!result)
            {
                return NotFound(new { message = "Unit not found" });
            }
            return NoContent();
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _unitService.DeleteUnitAsync(id);

            if (!result)
            {
                return NotFound(new { message = "Unit not found" });
            }
            return NoContent();
        }

        /// <summary>
        /// UpdateStatus
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromQuery] string status)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                return BadRequest(new { message = "Status is required" });
            }

            var result = await _unitService.UpdateUnitStatusAsync(id, status);

            if (!result)
            {
                return NotFound(new { message = "Unit not found" });
            }

            return Ok(new { message = $"Status updated to {status} successfully" });
        }
    }
}
