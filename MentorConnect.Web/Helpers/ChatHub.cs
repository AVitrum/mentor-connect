using MentorConnect.Data.Entities;
using MentorConnect.Data.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace MentorConnect.Web.Helpers;

public sealed class ChatHub : Hub
{
    private readonly IChatRepository _chatRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    public ChatHub(IChatRepository chatRepository, IMessageRepository messageRepository, UserManager<ApplicationUser> userManager)
    {
        _chatRepository = chatRepository;
        _messageRepository = messageRepository;
        _userManager = userManager;
    }

    public override async Task OnConnectedAsync()
    {
        await Clients.All.SendAsync("ReceiveMessage", "System", $"{Context.ConnectionId} joined the chat");
    }

    public async Task LoadChat(string receiverEmail)
    {
        ApplicationUser? sender = await _userManager.GetUserAsync(Context.User);
        ApplicationUser? receiver = await _userManager.FindByEmailAsync(receiverEmail);

        if (receiver is null)
        {
            await Clients.Caller.SendAsync("ReceiveMessage", "System", "User not found.");
            return;
        }

        Chat? chat = await _chatRepository.GetChatByUser1AndUser2(sender.Id, receiver.Id);

        if (chat != null)
        {
            foreach (Message chatMessage in chat.Messages)
            {
                await Clients.Caller.SendAsync("ReceiveMessage", chatMessage.Sender!.UserName, chatMessage.Content);
            }
        }
    }

    public async Task SendMessage(string receiverEmail, string message)
    {
        ApplicationUser? sender = await _userManager.GetUserAsync(Context.User);
        ApplicationUser? receiver = await _userManager.FindByEmailAsync(receiverEmail);

        if (receiver is null)
        {
            await Clients.Caller.SendAsync("ReceiveMessage", "System", "User not found.");
            return;
        }

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

        Message newMessage = new Message
        {
            ChatId = chat.Id,
            SenderId = sender.Id,
            ReceiverId = receiver.Id,
            Content = message,
        };

        chat.Messages.Add(newMessage);
        await _messageRepository.CreateAsync(newMessage);

        await Clients.User(receiver.Id).SendAsync("ReceiveMessage", newMessage.Sender!.UserName, newMessage.Content);
        await Clients.Caller.SendAsync("ReceiveMessage", newMessage.Sender.UserName, newMessage.Content);
    }
}