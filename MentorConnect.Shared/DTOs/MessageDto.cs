namespace MentorConnect.Shared.DTOs;

public class MessageDto
{
    public required string Sender { get; set; }
    public required string Content { get; set; }
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
}