namespace MentorConnect.Data.Interfaces;

public interface IRepository<T>
{
    Task CreateAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(Guid id);
    Task<T> GetByIdAsync(Guid id);
}