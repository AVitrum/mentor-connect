using System.ComponentModel.DataAnnotations;

namespace MentorConnect.Data.Entities;

public class Message
{
    [Key]
    public Guid Id { get; init; }
    
    public Guid ChatId { get; set; }
    public Chat? Chat { get; set; }
    
    public string SenderId { get; set; } 
    public ApplicationUser? Sender { get; set; }
    
    public string ReceiverId { get; set; }
    public ApplicationUser? Receiver { get; set; }
    
    public required string Content { get; set; }
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
}