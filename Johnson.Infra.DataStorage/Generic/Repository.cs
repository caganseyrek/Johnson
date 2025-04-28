using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Johnson.Infra.DataStorage.Generic;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly JohnsonDbContext _context;
    private readonly DbSet<T> _dbSet;

    public Repository(JohnsonDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public async Task<T?> GetEntityAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<ICollection<T>> GetAllEntitiesAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<ICollection<T>> GetEntitiesByAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }

    public async Task<T> CreateNewEntityAsync(T newEntity)
    {
        await _dbSet.AddAsync(newEntity);
        await _context.SaveChangesAsync();

        return newEntity;
    }

    public async Task<T?> UpdateEntityAsync(Guid id, T updatedEntity)
    {
        var existingEntry = await _dbSet.FindAsync(id);
        if (existingEntry is not null)
        {
            _dbSet.Entry(existingEntry).CurrentValues.SetValues(updatedEntity);
            await _context.SaveChangesAsync();

            return updatedEntity;
        }
        return null;
    }

    public async Task DeleteEntityAsync(Guid id)
    {
        var existingEntry = await _dbSet.FindAsync(id);
        if (existingEntry is not null)
        {
            _dbSet.Remove(existingEntry);
            await _context.SaveChangesAsync();
        }
    }
}
