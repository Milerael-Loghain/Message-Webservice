using System.Net.WebSockets;

namespace Service.Services.Abstract
{
    public interface IWebSocketService
    {
        public void AddClient(WebSocket socket);
        public void RemoveClient(WebSocket socket);
        public Task BroadcastMessage(object message);
    }
}