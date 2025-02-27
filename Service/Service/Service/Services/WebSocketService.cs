using System.Net.WebSockets;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using Service.Services.Abstract;

namespace Service.Services
{
    public class WebSocketService : IWebSocketService
    {
        private readonly ConcurrentDictionary<WebSocket, bool> _clients = new();

        public void AddClient(WebSocket socket)
        {
            _clients.TryAdd(socket, true);
        }

        public void RemoveClient(WebSocket socket)
        {
            _clients.TryRemove(socket, out _);
        }

        public async Task BroadcastMessage(object message)
        {
            var messageJson = JsonSerializer.Serialize(message);
            var buffer = Encoding.UTF8.GetBytes(messageJson);

            var closedSockets = new List<WebSocket>();

            foreach (var socket in _clients.Keys)
            {
                if (socket.State == WebSocketState.Open)
                {
                    await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
                }
                else
                {
                    closedSockets.Add(socket);
                }
            }

            // Clean up closed sockets
            foreach (var socket in closedSockets)
            {
                RemoveClient(socket);
            }
        }
    }
}