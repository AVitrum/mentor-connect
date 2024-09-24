namespace MentorConnect.Web.ViewModels;

public class ChatViewModel
{
    public string ReceiverEmail { get; set; }
    public string Message { get; set; }
    public List<string> Messages { get; set; } = [];
}