namespace PropertyManagementSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    //[AllowAnonymous]
    public class AuditController(IAuditService auditService) : ControllerBase
    {
        private readonly IAuditService _auditService = auditService;

        /// <summary>
        /// Gets paginated audit logs
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAuditLogs([FromQuery] int pageNumber = 1,[FromQuery] int pageSize = 50)
        {
            if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
            {
                return BadRequest("Invalid page number or page size.");
            }

            var logs = await _auditService.GetAuditLogsAsync(pageNumber, pageSize);

            return Ok(new
            {
                pageNumber,
                pageSize,
                totalItems = logs.Count(),
                items = logs
            });
        }

        /// <summary>
        /// Gets audit logs for a specific user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("user/{userId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAuditLogsByUser(Guid userId,[FromQuery] int pageNumber = 1,[FromQuery] int pageSize = 50)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest("Invalid user ID.");
            }

            if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
            {
                return BadRequest("Invalid page number or page size.");
            }

            var logs = await _auditService.GetAuditLogsByUserAsync(userId, pageNumber, pageSize);

            return Ok(new
            {
                userId,
                pageNumber,
                pageSize,
                totalItems = logs.Count(),
                items = logs
            });
        }

        /// <summary>
        /// Gets audit logs for a specific entity
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="entityId"></param>
        /// <returns></returns>
        [HttpGet("entity/{entityName}/{entityId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAuditLogsByEntity(string entityName, Guid entityId)
        {
            if (string.IsNullOrWhiteSpace(entityName))
            {
                return BadRequest("Entity name is required.");
            }

            if (entityId == Guid.Empty)
            {
                return BadRequest("Invalid entity ID.");
            }

            var logs = await _auditService.GetAuditLogsByEntityAsync(entityName, entityId);

            return Ok(new
            {
                entityName,
                entityId,
                totalItems = logs.Count(),
                items = logs
            });
        }

        /// <summary>
        /// Gets audit logs by action type
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        [HttpGet("action/{action}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAuditLogsByAction(string action)
        {
            if (string.IsNullOrWhiteSpace(action))
            {
                return BadRequest("Action is required.");
            }

            var logs = await _auditService.GetAuditLogsByActionAsync(action);

            return Ok(new
            {
                action,
                totalItems = logs.Count(),
                items = logs
            });
        }

        /// <summary>
        /// Gets audit logs within a date range
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [HttpGet("date-range")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAuditLogsByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            if (startDate > endDate)
            {
                return BadRequest("Start date must be before end date.");
            }

            var logs = await _auditService.GetAuditLogsByDateRangeAsync(startDate, endDate);

            return Ok(new
            {
                startDate,
                endDate,
                totalItems = logs.Count(),
                items = logs
            });
        }

        /// <summary>
        /// Creates a manual audit log entry
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateAuditLog([FromBody] CreateAuditLogDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _auditService.LogAsync(
                dto.Action,
                dto.EntityName,
                dto.EntityId,
                dto.UserId,
                dto.OldValue,
                dto.NewValue,
                dto.IpAddress,
                dto.UserAgent
            );

            return StatusCode(StatusCodes.Status201Created);
        }
    }
}