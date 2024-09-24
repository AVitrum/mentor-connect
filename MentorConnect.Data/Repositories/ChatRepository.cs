// Repositories/ChatRepository.cs
using MentorConnect.Data.Contexts;
using MentorConnect.Data.Entities;
using MentorConnect.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MentorConnect.Data.Repositories;

public class ChatRepository : Repository<Chat>, IChatRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ChatRepository(ApplicationDbContext dbContext) : base(dbContext, dbContext.Chats)
    {
        _dbContext = dbContext;
    }

    //TODO: Create exception class
    public async Task<Chat?> GetChatByUser1AndUser2(string senderId, string receiverId)
    {
        Chat? chat = await _dbContext.Chats
            .AsNoTracking()
            .Include(c => c.Messages)
                .ThenInclude(m => m.Sender)
            .Include(c => c.Messages)
                .ThenInclude(m => m.Receiver)
            .FirstOrDefaultAsync(c => 
                (c.User1Id == senderId && c.User2Id == receiverId) ||
                (c.User1Id == receiverId && c.User2Id == senderId));

        if (chat != null)
        {
            chat.Messages = chat.Messages.OrderBy(m => m.SentAt).ToList();
        }

        return chat;
    }



    public async Task<IEnumerable<Chat>> GetChatsAsync(string userId)
    {
        IEnumerable<Chat> chats = await _dbContext.Chats
            .Include(c => c.Messages)
            .Where(c => c.User1Id.ToString() == userId || c.User2Id.ToString() == userId)
            .ToListAsync();
        return chats;
    }
}