using MentorConnect.Shared.DTOs;

namespace MentorConnect.Web.Interfaces;

public interface IChatClient
{
    Task ReceiveMessage(MessageDto messageDto);
}