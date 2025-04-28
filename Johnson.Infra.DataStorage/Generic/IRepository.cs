using System.Linq.Expressions;

namespace Johnson.Infra.DataStorage.Generic;

public interface IRepository<T> where T : class
{
    public Task<T?> GetEntityAsync(Guid id);
    public Task<ICollection<T>> GetAllEntitiesAsync();
    public Task<ICollection<T>> GetEntitiesByAsync(Expression<Func<T, bool>> predicate);
    public Task<T> CreateNewEntityAsync(T newEntity);
    public Task<T?> UpdateEntityAsync(Guid id, T updatedEntity);
    public Task DeleteEntityAsync(Guid id);
}
