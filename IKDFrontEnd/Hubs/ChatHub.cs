using Microsoft.AspNetCore.SignalR;
using IKDFrontEnd.Models;
using Microsoft.EntityFrameworkCore;

namespace IKDFrontEnd.Hubs
{
    public class ChatHub : Hub
    {
        private static readonly HashSet<string> OnlineUsers = new();
        private readonly DbikdContext _context;

        public ChatHub(DbikdContext context)
        {
            _context = context;
        }

        public async Task SendMessage(string user, string message)
        {
            // Save message to DB
            var chatMsg = new ChatMessage
            {
                UserName = user,
                Message = message,
                SentAt = DateTime.Now
            };
            _context.ChatMessages.Add(chatMsg);
            await _context.SaveChangesAsync();

            // Send to all clients
            await Clients.All.SendAsync("ReceiveMessage", user, message, chatMsg.SentAt.ToString("HH:mm"));
        }

        public override async Task OnConnectedAsync()
        {
            var name = Context.GetHttpContext()?.Request.Query["username"].ToString();
            if (!string.IsNullOrEmpty(name))
            {
                OnlineUsers.Add(name);
                await Clients.All.SendAsync("UsersOnline", OnlineUsers);

                // Load last 30 messages from DB
                var messages = await _context.ChatMessages
                    .OrderByDescending(m => m.SentAt)
                    .Take(30)
                    .OrderBy(m => m.SentAt)
                    .ToListAsync();

                await Clients.Caller.SendAsync("LoadChatHistory", messages);
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var name = Context.GetHttpContext()?.Request.Query["username"].ToString();
            if (!string.IsNullOrEmpty(name))
            {
                OnlineUsers.Remove(name);
                await Clients.All.SendAsync("UsersOnline", OnlineUsers);
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
