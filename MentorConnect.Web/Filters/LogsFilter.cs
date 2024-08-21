namespace MentorConnect.Web.Filters;

public static class LogsFilter
{
    public static void AddLogsFilter(this ILoggingBuilder loggingBuilder)
    {
        loggingBuilder.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.None);
    }
}