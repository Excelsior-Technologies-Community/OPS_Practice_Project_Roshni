using Microsoft.AspNetCore.SignalR;

namespace OPS_Practice_Project.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string senderId, string message, bool isAdminReply)
        {
            await Clients.All.SendAsync("ReceiveMessage", senderId, message, isAdminReply, DateTime.Now.ToString("g"));

            if (!isAdminReply) 
            {
                await Clients.All.SendAsync("NewMessageNotification", senderId);
            }
        }
    }
}
