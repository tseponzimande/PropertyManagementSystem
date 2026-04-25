namespace PropertyManagementSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [AllowAnonymous]
    public class LeasesController(ILeaseService leaseService) : ControllerBase
    {
        private readonly ILeaseService _leaseService = leaseService;

        /// <summary>
        /// GetAll
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var leases = await _leaseService.GetAllLeasesAsync();
            return Ok(leases);
        }


        /// <summary>
        /// GetById
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var lease = await _leaseService.GetLeaseByIdAsync(id);

            if (lease == null)
            {
                return NotFound(new { message = "Lease not found" });
            }

            return Ok(lease);
        }

        /// <summary>
        /// GetByUnit
        /// </summary>
        /// <param name="unitId"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("unit/{unitId}")]
        public async Task<IActionResult> GetByUnit(Guid unitId)
        {
            var leases = await _leaseService.GetLeasesByUnitAsync(unitId);
            return Ok(leases);
        }

        /// <summary>
        /// GetByTenant
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("tenant/{tenantId}")]
        public async Task<IActionResult> GetByTenant(Guid tenantId)
        {
            var leases = await _leaseService.GetLeasesByTenantAsync(tenantId);
            return Ok(leases);
        }

        /// <summary>
        /// Create
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LeaseDto dto)
        {
            var lease = await _leaseService.CreateLeaseAsync(dto);
            if (lease == null)
            {
                return BadRequest(new { message = "Failed to create lease" });
            }

            return CreatedAtAction(nameof(GetById), new { id = lease.Id }, lease);
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] LeaseDto dto)
        {
            if (id != dto.Id)
            {
                return BadRequest(new { message = "ID mismatch" });
            }

            var updated = await _leaseService.UpdateLeaseAsync(dto);

            if (!updated)
            {
                return NotFound(new { message = "Lease not found" });
            }

            return NoContent();
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _leaseService.DeleteLeaseAsync(id);

            if (!deleted)
            {
                return NotFound(new { message = "Lease not found" });
            }

            return NoContent();
        }

        /// <summary>
        /// Terminate
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPatch("{id}/terminate")]
        public async Task<IActionResult> Terminate(Guid id)
        {
            var result = await _leaseService.TerminateLeaseAsync(id);

            if (!result)
            {
                return NotFound(new { message = "Lease not found" });
            }

            return Ok(new { message = "Lease terminated successfully" });
        }
    }
}
