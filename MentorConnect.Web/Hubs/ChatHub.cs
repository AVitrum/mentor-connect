using MentorConnect.Data.Entities;
using MentorConnect.Data.Interfaces;
using MentorConnect.Shared.DTOs;
using MentorConnect.Web.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace MentorConnect.Web.Hubs;

public sealed class ChatHub : Hub<IChatClient>
{
    private readonly IChatRepository _chatRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly ILogger<ChatHub> _logger;
    private readonly UserManager<ApplicationUser> _userManager;

    public ChatHub(IChatRepository chatRepository, IMessageRepository messageRepository, UserManager<ApplicationUser> userManager, ILogger<ChatHub> logger)
    {
        _chatRepository = chatRepository;
        _messageRepository = messageRepository;
        _logger = logger;
        _userManager = userManager;
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("Client connected: {ConnectionId}", Context.ConnectionId);
        MessageDto message = CreateBaseMessage("System", $"{Context.ConnectionId} joined the chat");
        await Clients.All.ReceiveMessage(message);
    }

    public async Task SendMessage(string receiverEmail, string message)
    {
        (ApplicationUser sender, ApplicationUser receiver) = await GetUsers(receiverEmail);
        Chat? chat = await _chatRepository.GetChatByUser1AndUser2(sender.Id, receiver.Id);

        if (chat == null)
        {
            chat = new Chat
            {
                User1Id = sender.Id,
                User2Id = receiver.Id,
                Messages = new List<Message>()
            };
            await _chatRepository.CreateAsync(chat);
        }

        var newMessage = new Message
        {
            ChatId = chat.Id,
            SenderId = sender.Id,
            ReceiverId = receiver.Id,
            Content = message,
            SentAt = DateTime.UtcNow
        };

        chat.Messages.Add(newMessage);
        await _messageRepository.CreateAsync(newMessage);
        
        MessageDto messageDto = CreateBaseMessage(sender.UserName!, message);
        
        await Clients.User(receiver.Id).ReceiveMessage(messageDto);
        await Clients.Caller.ReceiveMessage(messageDto);
    }

    public async Task LoadChat(string receiverEmail)
    {
        (ApplicationUser sender, ApplicationUser receiver) = await GetUsers(receiverEmail);
        Chat? chat = await _chatRepository.GetChatByUser1AndUser2(sender.Id, receiver.Id);

        if (chat != null)
        {
            foreach (Message chatMessage in chat.Messages)
            {
                MessageDto message = CreateBaseMessage(chatMessage.Sender!.UserName!, chatMessage.Content);
                await Clients.Caller.ReceiveMessage(message);
            }
        }
        else
        {
            MessageDto message = CreateBaseMessage("System", "There is no chat with this user.");
            await Clients.Caller.ReceiveMessage(message);
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("Client disconnected: {ConnectionId}", Context.ConnectionId);
        MessageDto message = CreateBaseMessage("System", $"{Context.ConnectionId} left the chat");
        await Clients.All.ReceiveMessage(message);
    }

    private MessageDto CreateBaseMessage(string sender, string content) =>
        new MessageDto { Sender = sender, Content = content };
    
    private async Task<(ApplicationUser, ApplicationUser)> GetUsers(string receiverEmail)
    {
        ApplicationUser? sender = await _userManager.GetUserAsync(Context.User!);
        ApplicationUser? receiver = await _userManager.FindByEmailAsync(receiverEmail);
        
        if (sender is null || receiver is null)
        {
            MessageDto message = CreateBaseMessage("System", "User not found.");
            await Clients.Caller.ReceiveMessage(message);
            
            //TODO: Create custom exception
            throw new Exception("User not found.");
        }

        return (sender, receiver);
    }
}