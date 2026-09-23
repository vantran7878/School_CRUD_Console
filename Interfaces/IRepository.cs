namespace School_CRUD_console.Interfaces;

public interface IRepository<T> where T : class
{
    Task<IReadOnlyList<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task DeleteAsync(T entity);
    Task ClearAsync();
}