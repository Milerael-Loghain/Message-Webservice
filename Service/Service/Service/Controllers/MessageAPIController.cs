using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Service.Data.Abstract;
using Service.DTO;
using Service.Extensions;
using Service.Services.Abstract;

namespace Service.Controllers
{
    [ApiController]
    [Route("api/messages")]
    public class MessageApiController : ControllerBase
    {
        private readonly IMessageDAL _messageDAL;
        private readonly IWebSocketService _webSocketService;
        private readonly ILogger<MessageApiController> _logger;

        public MessageApiController(IMessageDAL messageDAL, IWebSocketService webSocketService, ILogger<MessageApiController> logger)
        {
            _messageDAL = messageDAL;
            _webSocketService = webSocketService;
            _logger = logger;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageDTO messageDto)
        {
            _logger.LogInformation($"Received SendMessage request with text: {messageDto.Text}, id: {messageDto.InternalId}");

            if (string.IsNullOrWhiteSpace(messageDto.Text))
            {
                _logger.LogWarning("Message text is empty or null.");
                return BadRequest("Invalid message data.");
            }

            if (messageDto.Text.Length > 128)
            {
                _logger.LogWarning("Message text exceeds the 128-character limit.");
                return BadRequest("Message exceeds the 128-character limit.");
            }

            var message = messageDto.ToMessage(DateTime.Now);
            _logger.LogInformation($"Message created with dateTime: ({DateTime.Now})");

            try
            {
                await _messageDAL.InsertMessage(message.InternalId, message.Text);
                _logger.LogInformation($"Message inserted into the database.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while inserting message into the database.");
                return StatusCode(500, "An error occurred while processing your request.");
            }

            try
            {
                await _webSocketService.BroadcastMessage(message);
                _logger.LogInformation("Message broadcasted to all websocket clients.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while broadcasting message.");
                return StatusCode(500, "An error occurred while broadcasting the message.");
            }

            return Ok();
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetMessagesByDateRange([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            _logger.LogInformation($"Received GetMessagesByDateRange request from: {from}, to: {to}");

            if (from > to)
            {
                _logger.LogWarning($"Invalid date range: from ({from}) is later than to ({to}).");
                return BadRequest("Invalid date range: 'from' must be earlier than 'to'.");
            }

            try
            {
                var messages = await _messageDAL.GetMessagesInRangeAsync(from, to);
                _logger.LogInformation($"Retrieved {messages.Count} messages from the database.");
                return Ok(messages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching messages from the database.");
                return StatusCode(500, "An error occurred while fetching the message history.");
            }
        }

    }
}