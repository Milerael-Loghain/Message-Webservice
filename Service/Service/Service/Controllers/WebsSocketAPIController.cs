using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using Service.Services.Abstract;

[ApiController]
[Route("ws")]
public class WebsSocketAPIController : ControllerBase
{
    private readonly IWebSocketService _webSocketService;

    public WebsSocketAPIController(IWebSocketService webSocketService)
    {
        _webSocketService = webSocketService;
    }

    [HttpGet("messages")]
    public async Task GetWebSocket()
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            using var socket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            _webSocketService.AddClient(socket);

            var buffer = new byte[1024 * 4];
            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close)
                    break;
            }

            _webSocketService.RemoveClient(socket);
            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
        }
        else
        {
            HttpContext.Response.StatusCode = 400;
        }
    }
}