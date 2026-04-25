namespace PropertyManagementSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    [AllowAnonymous]
    public class DashboardController(IDashboardService dashboardService, ILogger<DashboardController> logger) : ControllerBase
    {
        private readonly IDashboardService _dashboardService = dashboardService;
        private readonly ILogger<DashboardController> _logger = logger;

        /// <summary>
        ///  Admin users
        /// </summary>
        /// <returns></returns>
        [HttpGet("admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAdminDashboard()
        {
            try
            {
                var stats = await _dashboardService.GetAdminDashboardStatsAsync();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving admin dashboard");
                return StatusCode(500, new { message = "Failed to retrieve dashboard statistics" });
            }
        }


        /// <summary>
        /// Owner
        /// </summary>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        [HttpGet("owner/{ownerId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetOwnerDashboard(Guid ownerId)
        {
            try
            {
                if (ownerId == Guid.Empty)
                {
                    return BadRequest(new { message = "Invalid owner ID" });
                }
                var stats = await _dashboardService.GetOwnerDashboardStatsAsync(ownerId);
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving owner dashboard for {OwnerId}", ownerId);
                return StatusCode(500, new { message = "Failed to retrieve dashboard statistics" });
            }
        }


        /// <summary>
        /// Tenant
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        [HttpGet("tenant/{tenantId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTenantDashboard(Guid tenantId)
        {
            try
            {
                if (tenantId == Guid.Empty)
                {
                    return BadRequest(new { message = "Invalid tenant ID" });
                }

                var stats = await _dashboardService.GetTenantDashboardStatsAsync(tenantId);
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tenant dashboard for {TenantId}", tenantId);
                return StatusCode(500, new { message = "Failed to retrieve dashboard statistics" });
            }
        }

        /// <summary>
        /// Gets revenue statistics (Admin sees all, Owner sees their properties only)
        /// </summary>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        [HttpGet("revenue")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetRevenueStats([FromQuery] Guid? ownerId = null)
        {
            try
            {
                var stats = await _dashboardService.GetRevenueStatsAsync(ownerId);
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving revenue stats");
                return StatusCode(500, new { message = "Failed to retrieve revenue statistics" });
            }
        }

        /// <summary>
        /// Gets property-level statistics with occupancy rates
        /// </summary>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        [HttpGet("properties")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPropertyStats([FromQuery] Guid? ownerId = null)
        {
            try
            {
                var stats = await _dashboardService.GetPropertyStatsAsync(ownerId);
                return Ok(new
                {
                    propertyCount = stats.Count(),
                    properties = stats
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving property stats");
                return StatusCode(500, new { message = "Failed to retrieve property statistics" });
            }
        }


        /// <summary>
        /// Gets maintenance request statistics by status
        /// </summary>
        /// <returns></returns>
        [HttpGet("maintenance")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetMaintenanceStats()
        {
            try
            {
                var stats = await _dashboardService.GetMaintenanceStatsAsync();
                return Ok(new
                {
                    totalRequests = stats.Sum(s => s.Count),
                    breakdown = stats
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving maintenance stats");
                return StatusCode(500, new { message = "Failed to retrieve maintenance statistics" });
            }
        }

        /// <summary>
        /// Gets a summary dashboard for quick overview (all roles)
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        [HttpGet("summary/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetDashboardSummary(Guid userId, [FromQuery] string role)
        {
            try
            {
                if (userId == Guid.Empty)
                {
                    return BadRequest(new { message = "Invalid user ID" });
                }

                if (string.IsNullOrWhiteSpace(role))
                {
                    return BadRequest(new { message = "Role is required" });
                }

                var stats = role.ToLower() switch
                {
                    "admin" => await _dashboardService.GetAdminDashboardStatsAsync(),
                    "owner" => await _dashboardService.GetOwnerDashboardStatsAsync(userId),
                    "tenant" => await _dashboardService.GetTenantDashboardStatsAsync(userId),
                    _ => throw new ArgumentException("Invalid role specified")
                };

                return Ok(new
                {
                    userId,
                    role,
                    summary = stats
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving dashboard summary for user {UserId}", userId);
                return StatusCode(500, new { message = "Failed to retrieve dashboard summary" });
            }
        }
    }
}