namespace PropertyManagementSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    [AllowAnonymous]
    public class MaintenanceRequestsController(IMaintenanceService maintenanceService) : ControllerBase
    {
        private readonly IMaintenanceService _maintenanceService = maintenanceService;

        /// <summary>
        /// Retrieves all maintenance requests for a specific unit.
        /// </summary>
        /// <param name="unitId"></param>
        /// <returns></returns>
        [HttpGet("unit/{unitId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByUnit(Guid unitId)
        {
            var requests = await _maintenanceService.GetRequestsByUnitAsync(unitId);
            return Ok(requests);
        }

        /// <summary>
        ///  Creates a new maintenance request.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] MaintenanceRequestDto dto)
        {
            var request = await _maintenanceService.CreateRequestAsync(dto);

            if (request == null)
            {
                return BadRequest(new { message = "Failed to create maintenance request" });
            }

            return Ok(request);
        }

        /// <summary>
        /// Updates the status of a maintenance request.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpPatch("{id}/status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromQuery] string status)
        {
            var updated = await _maintenanceService.UpdateRequestStatusAsync(id, status);

            if (!updated)
            {
                return NotFound(new { message = "Maintenance request not found" });
            }

            return Ok(new { message = "Status updated successfully" });
        }
    }
}
