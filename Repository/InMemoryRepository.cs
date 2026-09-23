namespace School_CRUD_console.Repository;

using School_CRUD_console.Interfaces;

public class InMemoryRepository<T> : IRepository<T> where T : class
{
    protected readonly List<T> _data = new();

    public InMemoryRepository() { }
    public Task<IReadOnlyList<T>> GetAllAsync()
    {
        return Task.FromResult<IReadOnlyList<T>>(_data.AsReadOnly());
    }

    public Task AddAsync(T entity)
    {
        _data.Add(entity);
        return Task.CompletedTask;
    }
    public Task DeleteAsync(T entity)
    {
        _data.Remove(entity);
        return Task.CompletedTask;
    }

    public Task ClearAsync()
    {
        _data.Clear();
        return Task.CompletedTask;
    }
}