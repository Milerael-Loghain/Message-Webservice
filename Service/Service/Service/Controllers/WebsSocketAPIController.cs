using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using Service.Services.Abstract;

[ApiController]
[Route("ws")]
public class WebsSocketAPIController : ControllerBase
{
    private readonly IWebSocketService _webSocketService;
    private readonly ILogger<WebsSocketAPIController> _logger;

    public WebsSocketAPIController(IWebSocketService webSocketService, ILogger<WebsSocketAPIController> logger)
    {
        _webSocketService = webSocketService;
        _logger = logger;
    }

    [HttpGet("messages")]
    public async Task GetWebSocket()
    {
        _logger.LogInformation("Received WebSocket connection request.");

        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            try
            {
                _logger.LogInformation("Accepting WebSocket connection.");

                using var socket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                _webSocketService.AddClient(socket);
                _logger.LogInformation("WebSocket connection established.");

                var buffer = new byte[1024 * 4];
                while (socket.State == WebSocketState.Open)
                {
                    var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        _logger.LogInformation("WebSocket connection closed by client.");
                        break;
                    }
                }

                _webSocketService.RemoveClient(socket);
                _logger.LogInformation("WebSocket client removed and connection closed.");
                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while handling WebSocket connection.");
                HttpContext.Response.StatusCode = 500;
            }
        }
        else
        {
            _logger.LogWarning("Invalid WebSocket request. Responding with status 400.");
            HttpContext.Response.StatusCode = 400;
        }
    }
}