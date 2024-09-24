using MentorConnect.Web.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace MentorConnect.Web.Hubs;

public class NotificationHub : Hub<INotificationClient>
{
    public override async Task OnConnectedAsync()
    {
        await Clients.Client(Context.ConnectionId).ReceiveNotification(
            $"Welcome to MentorConnect! Your connection id is {Context.ConnectionId}");
        
        await base.OnConnectedAsync();
    }
}

