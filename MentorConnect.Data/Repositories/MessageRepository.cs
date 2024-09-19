using MentorConnect.Data.Contexts;
using MentorConnect.Data.Entities;
using MentorConnect.Data.Interfaces;

namespace MentorConnect.Data.Repositories;

public class MessageRepository : Repository<Message>, IMessageRepository
{
    private readonly ApplicationDbContext _dbContext;
    
    public MessageRepository(ApplicationDbContext dbContext) : base(dbContext, dbContext.Messages)
    {
        _dbContext = dbContext;
    }
}