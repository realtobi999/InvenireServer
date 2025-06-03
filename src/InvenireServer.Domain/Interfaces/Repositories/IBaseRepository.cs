using System.Linq.Expressions;

namespace InvenireServer.Domain.Interfaces.Repositories;

/// <summary>
/// Defines a contract for generic repository for basic CRUD operations on entities of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The entity type.</typeparam>
public interface IBaseRepository<T> where T : class
{
    /// <summary>
    /// Retrieves all entities of type <c><typeparamref name="T"/></c>.
    /// </summary>
    /// <returns>
    /// A task representing the asynchronous operation. The task result is an enumerable collection of type <c><typeparamref name="T"/></c>.
    /// </returns>
    Task<IEnumerable<T>> IndexAsync();

    /// <summary>
    /// Retrieves entities of type <typeparamref name="T"/> that match the specified predicate.
    /// </summary>
    /// <param name="predicate">An expression to filter entities.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The task result is an enumerable collection of type <c><typeparamref name="T"/></c>that match the predicate.
    /// </returns>
    Task<IEnumerable<T>> IndexAsync(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// Retrieves a single entity of type <typeparamref name="T"/> that matches the specified predicate.
    /// </summary>
    /// <param name="predicate">An expression to filter the entity.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The task result is the matching entity of type <c><typeparamref name="T"/></c>, or <c>null</c> if no match is found.
    /// </returns>
    Task<T?> GetAsync(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// Creates a new entity of type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="entity">The entity of type <c><typeparamref name="T"/></c> to create.</param>
    void Create(T entity);

    /// <summary>
    /// Updates an existing entity of type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="entity">The entity of type <c><typeparamref name="T"/></c> to update.</param>
    void Update(T entity);

    /// <summary>
    /// Deletes the specified entity of type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="entity">The entity of type <c><typeparamref name="T"/></c> to delete.</param>
    void Delete(T entity);
}