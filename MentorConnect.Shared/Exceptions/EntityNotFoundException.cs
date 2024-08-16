namespace MentorConnect.Shared.Exceptions;

public class EntityNotFoundException<T> : Exception
{
    public EntityNotFoundException() : base($"{typeof(T).Name} was not found.") { }
    public EntityNotFoundException(string message) : base($"{typeof(T).Name} was not found. {message}") { }
    public EntityNotFoundException(string message, Exception exception) : base($"{typeof(T).Name} was not found. {message}", exception) { }
}