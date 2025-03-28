using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace OPS_Practice_Project.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(int senderId, int receiverId, string message, bool isAdminReply)
        {
            // Broadcast the message to the receiver
            await Clients.User(receiverId.ToString()).SendAsync("ReceiveMessage", senderId, receiverId, message, isAdminReply, DateTime.Now.ToString("hh:mm tt"));

            // Notify Admin (If the message is from a user)
            if (!isAdminReply)
            {
                await Clients.All.SendAsync("ShowNotification", senderId);
            }
        }

    }
}
