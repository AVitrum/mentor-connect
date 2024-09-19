using MentorConnect.Data.Entities;

namespace MentorConnect.Data.Interfaces;

public interface IChatRepository : IRepository<Chat>
{
    Task<Chat?> GetChatByUser1AndUser2(string senderId, string receiverId);
    Task<IEnumerable<Chat>> GetChatsAsync(string userId);
}