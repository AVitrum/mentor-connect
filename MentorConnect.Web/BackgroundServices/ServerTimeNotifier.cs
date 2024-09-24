using MentorConnect.Web.Hubs;
using MentorConnect.Web.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace MentorConnect.Web.BackgroundServices;

public class ServerTimeNotifier : BackgroundService
{
    private static readonly TimeSpan Period = TimeSpan.FromSeconds(5);
    private readonly ILogger<ServerTimeNotifier> _logger;
    private readonly IHubContext<NotificationHub, INotificationClient> _hubContext;

    public ServerTimeNotifier(ILogger<ServerTimeNotifier> logger, IHubContext<NotificationHub, INotificationClient> hubContext)
    {
        _logger = logger;
        _hubContext = hubContext;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using PeriodicTimer timer = new PeriodicTimer(Period);

        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            DateTime dateTime = DateTime.Now;
            
            _logger.LogInformation("Executing {Service} {Time}", nameof(ServerTimeNotifier), dateTime);
            
            await _hubContext.Clients.All.ReceiveNotification($"Server time is {dateTime}");
        }
    }
}