using System.ComponentModel.DataAnnotations;

namespace MentorConnect.Data.Entities;

public class Chat
{
    [Key]
    public Guid Id { get; set; }
    
    public required string User1Id { get; set; }
    public ApplicationUser? User1 { get; set; }
    public required string User2Id { get; set; }
    public ApplicationUser? User2 { get; set; }
    
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}