using MentorConnect.Data.Contexts;
using MentorConnect.Data.Interfaces;
using MentorConnect.Shared.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace MentorConnect.Data.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly ApplicationDbContext _dbContext;
    private readonly DbSet<T> _dbSet;
    
    public Repository(ApplicationDbContext dbContext, DbSet<T> dbSet)
    {
        _dbContext = dbContext;
        _dbSet = dbSet;
    }
    
    public virtual async Task CreateAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _dbContext.SaveChangesAsync();
    }

    public virtual async Task UpdateAsync(T entity)
    {
        _dbSet.Attach(entity);
        _dbContext.Entry(entity).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();
    }

    public virtual async Task DeleteAsync(Guid id)
    {
        var entity = await GetByIdAsync(id);
        _dbSet.Remove(entity);
        await _dbContext.SaveChangesAsync();
    }

    public virtual async Task<T> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id) ?? throw new EntityNotFoundException<T>();
    }
}