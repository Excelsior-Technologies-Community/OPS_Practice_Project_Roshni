using Microsoft.AspNetCore.SignalR;
using OPS_Practice_Project.Models;
using OPS_Practice_Project.Repository;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace OPS_Practice_Project.Hubs
{
    public class ChatHub : Hub
    {
        private static ConcurrentDictionary<int, string> ConnectedUsers = new ConcurrentDictionary<int, string>();
        private readonly ChatRepository _chatRepository;

        public ChatHub(ChatRepository chatRepository)
        {
            _chatRepository = chatRepository;
        }

        // ✅ Track user connections
        public override async Task OnConnectedAsync()
        {
            var userId = Context.GetHttpContext()?.User?.FindFirst("UserId")?.Value;
            if (int.TryParse(userId, out int id) && id > 0)
            {
                ConnectedUsers[id] = Context.ConnectionId;
            }
            await Clients.All.SendAsync("UpdateUserList", ConnectedUsers.Keys);
            await base.OnConnectedAsync();
        }

        // ✅ Handle user disconnection
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var disconnectedUser = ConnectedUsers.FirstOrDefault(x => x.Value == Context.ConnectionId);
            if (disconnectedUser.Key != 0)
            {
                ConnectedUsers.TryRemove(disconnectedUser.Key, out _);
            }
            await Clients.All.SendAsync("UpdateUserList", ConnectedUsers.Keys);
            await base.OnDisconnectedAsync(exception);
        }

        // ✅ Send message & save to DB
        public async Task SendMessage(int senderId, int receiverId, string message, bool isAdminReply)
        {
            if (string.IsNullOrWhiteSpace(message)) return;

            // Save message to the database
            var chatMessage = new ChatModel
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Message = message,
                MessageType = "Text",
                IsAdminReply = isAdminReply,
                IsRead = false,
                SentOn = DateTime.Now
            };
            _chatRepository.InsertChat(chatMessage, "Insert");

            string timestamp = DateTime.Now.ToString("hh:mm tt");

            // ✅ If the receiver is connected, send the message in real time
            if (ConnectedUsers.TryGetValue(receiverId, out string receiverConnectionId))
            {
                await Clients.Client(receiverConnectionId).SendAsync("ReceiveMessage", senderId, receiverId, message, isAdminReply, timestamp);
            }

            // ✅ Notify admin/users about unread messages
            if (!isAdminReply)
            {
                await Clients.All.SendAsync("ShowNotification", senderId);
            }
        }
    }
}
