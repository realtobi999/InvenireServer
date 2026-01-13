using System.Linq.Expressions;
using InvenireServer.Domain.Entities.Common.Queries;

namespace InvenireServer.Application.Interfaces.Repositories;

/// <summary>
/// Defines core repository operations for an entity.
/// </summary>
/// <typeparam name="TEntity">Entity type.</typeparam>
public interface IRepositoryBase<TEntity> where TEntity : class
{
    /// <summary>
    /// Counts entities matching a predicate.
    /// </summary>
    /// <param name="predicate">Filter predicate.</param>
    /// <returns>Awaitable task returning the number of matching entities.</returns>
    Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// Counts entities matching all provided predicates.
    /// </summary>
    /// <param name="predicates">Filter predicates.</param>
    /// <returns>Awaitable task returning the number of matching entities.</returns>
    Task<int> CountAsync(IEnumerable<Expression<Func<TEntity, bool>>> predicates);

    /// <summary>
    /// Returns entities matching a predicate.
    /// </summary>
    /// <param name="predicate">Filter predicate.</param>
    /// <returns>Awaitable task returning matching entities.</returns>
    Task<IEnumerable<TEntity>> IndexAsync(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// Returns entities using query options.
    /// </summary>
    /// <typeparam name="TResult">Result type.</typeparam>
    /// <param name="options">Query options.</param>
    /// <returns>Awaitable task returning the query results.</returns>
    Task<IEnumerable<TResult>> IndexAsync<TResult>(QueryOptions<TEntity, TResult> options);

    /// <summary>
    /// Returns the first entity matching a predicate.
    /// </summary>
    /// <param name="predicate">Filter predicate.</param>
    /// <returns>Awaitable task returning the entity or null.</returns>
    Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// Returns an entity using query options.
    /// </summary>
    /// <typeparam name="TResult">Result type.</typeparam>
    /// <param name="options">Query options.</param>
    /// <returns>Awaitable task returning the entity or null.</returns>
    Task<TResult?> GetAsync<TResult>(QueryOptions<TEntity, TResult> options);

    /// <summary>
    /// Adds an entity to the repository.
    /// </summary>
    /// <param name="entity">Entity to add.</param>
    void Create(TEntity entity);

    /// <summary>
    /// Adds an entity and persists the change.
    /// </summary>
    /// <param name="entity">Entity to add.</param>
    /// <returns>Awaitable task representing the create operation.</returns>
    Task ExecuteCreateAsync(TEntity entity);

    /// <summary>
    /// Updates an entity in the repository.
    /// </summary>
    /// <param name="entity">Entity to update.</param>
    void Update(TEntity entity);

    /// <summary>
    /// Updates multiple entities in the repository.
    /// </summary>
    /// <param name="entities">Entities to update.</param>
    void Update(IEnumerable<TEntity> entities);

    /// <summary>
    /// Updates an entity and persists the change.
    /// </summary>
    /// <param name="entity">Entity to update.</param>
    /// <returns>Awaitable task representing the update operation.</returns>
    Task ExecuteUpdateAsync(TEntity entity);

    /// <summary>
    /// Deletes an entity from the repository.
    /// </summary>
    /// <param name="entity">Entity to delete.</param>
    void Delete(TEntity entity);

    /// <summary>
    /// Deletes an entity and persists the change.
    /// </summary>
    /// <param name="entity">Entity to delete.</param>
    /// <returns>Awaitable task representing the delete operation.</returns>
    Task ExecuteDeleteAsync(TEntity entity);

    /// <summary>
    /// Deletes entities matching a predicate.
    /// </summary>
    /// <param name="predicate">Filter predicate.</param>
    /// <returns>Awaitable task representing the delete operation.</returns>
    Task ExecuteDeleteWhereAsync(Expression<Func<TEntity, bool>> predicate);
}
