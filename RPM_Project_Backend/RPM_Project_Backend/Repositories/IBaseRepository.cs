using System.Linq.Expressions;

namespace RPM_Project_Backend.Repositories;

public interface IBaseRepository<TModel> where TModel : class
{
    /// <summary>
    /// Add item
    /// </summary>
    /// <param name="model">item</param>
    /// <returns>input item</returns>
    Task<TModel> CreateAsync(TModel model);
    /// <summary>
    /// Update item
    /// </summary>
    /// <param name="model">item</param>
    /// <returns>updated item</returns>
    Task<TModel> UpdateAsync(TModel model);
    /// <summary>
    /// Remove item by id
    /// </summary>
    /// <param name="id">item's id</param>
    /// <returns>removed item</returns>
    Task<TModel> DeleteAsync(long id);
    /// <summary>
    /// Remove item
    /// </summary>
    /// <param name="item">item</param>
    /// <returns>removed item</returns>
    Task<TModel> DeleteAsync(TModel item);
    /// <summary>
    /// Get all items
    /// </summary>
    /// <returns>item</returns>
    Task<IEnumerable<TModel>> GetAllAsync();
    /// <summary>
    /// Get items with where predicate
    /// </summary>
    /// <param name="predicate">Where predicate</param>
    /// <returns>items</returns>
    IEnumerable<TModel> GetWhereAsync(Func<TModel, bool> predicate);
    /// <summary>
    /// Get item by id
    /// </summary>
    /// <param name="id">item id</param>
    /// <returns>item</returns>
    Task<TModel> GetAsync(long id);
    /// <summary>
    /// Get children items
    /// </summary>
    /// <param name="includeProperties">Include properties</param>
    /// <returns>items</returns>
    IEnumerable<TModel> GetWithInclude(params Expression<Func<TModel, object>>[] includeProperties);
    /// <summary>
    /// Get items with where predicate and include properties
    /// </summary>
    /// <param name="predicate">Where predicate</param>
    /// <param name="includeProperties">Include properties</param>
    /// <returns>items</returns>
    IEnumerable<TModel> GetWithInclude(Func<TModel, bool> predicate, params Expression<Func<TModel, object>>[] includeProperties);
    /// <summary>
    /// Get items with include properties
    /// </summary>
    /// <param name="includeProperties">Inlcude properties</param>
    /// <returns>items</returns>
    IQueryable<TModel> Include(params Expression<Func<TModel, object>>[] includeProperties);
}