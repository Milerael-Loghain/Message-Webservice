using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Service.Data.Abstract;
using Service.Hubs;
using Service.Model;

namespace Service.Controllers
{
    [ApiController]
    [Route("api/messages")]
    public class MessageApiController : ControllerBase
    {
        private readonly IMessageDAL _messageDAL;
        private readonly IHubContext<MessageHub> _hubContext;

        public MessageApiController(IMessageDAL messageDAL, IHubContext<MessageHub> hubContext)
        {
            _messageDAL = messageDAL;
            _hubContext = hubContext;
        }

        [HttpPost("send")]
        public IActionResult SendMessage([FromBody] Message message)
        {
            if (string.IsNullOrWhiteSpace(message.Text))
            {
                return BadRequest("Invalid message data.");
            }

            if (message.Text.Length > 128)
            {
                return BadRequest("Message exceeds the 128-character limit.");
            }

            _messageDAL.InsertMessage(message.InternalId, message.Text);

            _hubContext.Clients.All.SendAsync("ReceiveMessage", message);

            return Ok();
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetMessagesByDateRange([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            if (from > to)
            {
                return BadRequest("Invalid date range: 'from' must be earlier than 'to'.");
            }

            var messages = await _messageDAL.GetMessagesInRangeAsync(from, to);
            return Ok(messages);
        }

    }
}