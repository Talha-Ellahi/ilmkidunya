using Microsoft.AspNetCore.SignalR;
using IKDFrontEnd.Models;
using Microsoft.EntityFrameworkCore;

namespace IKDFrontEnd.Hubs
{
    // We will rely on Context.UserIdentifier = userId string set from cookie claims
    public class PrivateChatHub : Hub
    {
        private readonly DbikdContext _context;

        public PrivateChatHub(DbikdContext context)
        {
            _context = context;
        }

        // Called from client to send a private message
        public async Task SendPrivateMessage(int senderId, int receiverId, string message)
        {
            // Save message in DB
            var pm = new PrivateMessage
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Message = message,
                SentAt = DateTime.UtcNow
            };

            _context.PrivateMessages.Add(pm);
            await _context.SaveChangesAsync();

            var time = pm.SentAt.ToString("HH:mm");

            // Notify both sender and receiver (Clients.User uses UserIdentifier)
            await Clients.Users(senderId.ToString(), receiverId.ToString())
                .SendAsync("ReceivePrivateMessage", new
                {
                    id = pm.Id,
                    senderId,
                    receiverId,
                    message = pm.Message,
                    sentAt = time
                });
        }

        // Optional: load chat history for a pair (we provide method server-invokable)
        public async Task LoadHistory(int userId, int friendId, int take = 50)
        {
            var msgs = await _context.PrivateMessages
                .Where(m => m.SenderId == userId && m.ReceiverId == friendId ||
                            m.SenderId == friendId && m.ReceiverId == userId)
                .OrderBy(m => m.SentAt)
                .Take(take)
                .Select(m => new {
                    id = m.Id,
                    senderId = m.SenderId,
                    receiverId = m.ReceiverId,
                    message = m.Message,
                    sentAt = m.SentAt.ToString("HH:mm")
                })
                .ToListAsync();

            await Clients.Caller.SendAsync("LoadHistoryResult", msgs);
        }

        public override Task OnConnectedAsync()
        {
            // nothing special here — UserIdentifier must be set in cookie claims (see AccountController)
            return base.OnConnectedAsync();
        }
    }
}
