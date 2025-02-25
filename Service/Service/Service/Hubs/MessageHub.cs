using Microsoft.AspNetCore.SignalR;

namespace Service.Hubs
{
    public class MessageHub : Hub
    {
        public async Task SendMessageToAllClients(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }
    }
}