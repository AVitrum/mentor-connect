namespace MentorConnect.Web.Interfaces;

public interface INotificationClient
{
    Task ReceiveNotification(string message);
}