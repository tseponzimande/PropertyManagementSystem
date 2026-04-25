namespace PropertyManagementSystem.Api.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class ChatController(IChatService chatService) : ControllerBase
    {
        private readonly IChatService _chatService = chatService;

        /// <summary>
        /// Get Conversation
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="otherUserId"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("conversation/{userId}/{otherUserId}")]
        public async Task<IActionResult> GetConversation(Guid userId, Guid otherUserId)
        {
            var conversation = await _chatService.GetConversationAsync(userId, otherUserId);
            return Ok(conversation);
        }


        /// <summary>
        /// Get User Conversations
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("conversations/{userId}")]
        public async Task<IActionResult> GetUserConversations(Guid userId)
        {
            var conversations = await _chatService.GetUserConversationsAsync(userId);
            return Ok(conversations);
        }

        /// <summary>
        /// SendMessage
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] MessageDto dto)
        {
            var message = await _chatService.SendMessageAsync(dto);
            return message != null
                ? CreatedAtAction(nameof(GetConversation), new { userId = dto.SenderId, otherUserId = dto.ReceiverId }, message)
                : BadRequest("Failed to send message");
        }


        [HttpPost("send-with-attachment")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SendMessageWithAttachment(
    [FromForm] SendMessageWithAttachmentDto dto)
        {
            if (dto.Attachment == null || dto.Attachment.Length == 0)
                return BadRequest("Attachment file is required");

            var messageDto = new MessageDto
            {
                SenderId = dto.SenderId,
                ReceiverId = dto.ReceiverId,
                Content = dto.Content
            };

            var message = await _chatService.SendMessageWithAttachmentAsync(
                messageDto,
                dto.Attachment);

            return message != null
                ? CreatedAtAction(
                    nameof(GetConversation),
                    new { userId = dto.SenderId, otherUserId = dto.ReceiverId },
                    message)
                : BadRequest("Failed to send message with attachment");
        }


        /// <summary>
        /// Mark Message As Read
        /// </summary>
        /// <param name="messageId"></param>
        /// <returns></returns>
        [HttpPatch("{messageId}/mark-read")]
        public async Task<IActionResult> MarkMessageAsRead(Guid messageId)
        {
            var success = await _chatService.MarkMessageAsReadAsync(messageId);
            return success ? Ok() : NotFound();
        }

        /// <summary>
        /// Mark Conversation As Read
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="otherUserId"></param>
        /// <returns></returns>
        [HttpPatch("conversation/{userId}/{otherUserId}/mark-read")]
        public async Task<IActionResult> MarkConversationAsRead(Guid userId, Guid otherUserId)
        {
            var success = await _chatService.MarkConversationAsReadAsync(userId, otherUserId);
            return success ? Ok() : BadRequest();
        }

        /// <summary>
        /// Get Unread Message Count
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("unread-count/{userId}")]
        public async Task<IActionResult> GetUnreadMessageCount(Guid userId)
        {
            var count = await _chatService.GetUnreadMessageCountAsync(userId);
            return Ok(new { userId, unreadCount = count });
        }

        /// <summary>
        /// Search Messages
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        [HttpGet("search/{userId}")]
        public async Task<IActionResult> SearchMessages(Guid userId, [FromQuery] string searchTerm)
        {
            var result = await _chatService.SearchMessagesAsync(userId, searchTerm);
            return Ok(result);
        }

        /// <summary>
        /// Delete Message
        /// </summary>
        /// <param name="messageId"></param>
        /// <returns></returns>
        [HttpDelete("{messageId}")]
        public async Task<IActionResult> DeleteMessage(Guid messageId)
        {
            var success = await _chatService.DeleteMessageAsync(messageId);
            return success ? NoContent() : NotFound();
        }

        /// <summary>
        /// Delete Conversation
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="otherUserId"></param>
        /// <returns></returns>
        [HttpDelete("conversation/{userId}/{otherUserId}")]
        public async Task<IActionResult> DeleteConversation(Guid userId, Guid otherUserId)
        {
            var success = await _chatService.DeleteConversationAsync(userId, otherUserId);
            return success ? NoContent() : BadRequest();
        }
    }
}
