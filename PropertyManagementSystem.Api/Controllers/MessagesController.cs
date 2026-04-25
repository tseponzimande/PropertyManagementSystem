namespace PropertyManagementSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    [AllowAnonymous]
    public class MessagesController(IMessageService messageService) : ControllerBase
    {
        private readonly IMessageService _messageService = messageService;

        /// <summary>
        /// GetBySender
        /// </summary>
        /// <param name="senderId"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("sender/{senderId}")]
        public async Task<IActionResult> GetBySender(Guid senderId)
        {
            var messages = await _messageService.GetMessagesBySenderAsync(senderId);
            return Ok(messages);
        }


        /// <summary>
        /// GetByReceiver
        /// </summary>
        /// <param name="receiverId"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("receiver/{receiverId}")]
        public async Task<IActionResult> GetByReceiver(Guid receiverId)
        {
            var messages = await _messageService.GetMessagesByReceiverAsync(receiverId);
            return Ok(messages);
        }

        /// <summary>
        /// Send
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost]
        public async Task<IActionResult> Send([FromBody] MessageDto dto)
        {
            var message = await _messageService.SendMessageAsync(dto);
            if (message == null)
                return BadRequest(new { message = "Failed to send message" });

            return Ok(message);
        }
    }
}
