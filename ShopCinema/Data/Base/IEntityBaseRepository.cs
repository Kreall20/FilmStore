using ShopCinema.Data.Base;
using ShopCinema.Models;
using System.Linq.Expressions;

namespace ShopCinema.Data.Base
{
    public interface IEntityBaseRepository<T> where T: class,IEntityBase, new()
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id, params Expression<Func<T, object>>[] includeProperties);
        Task<T> GetByIDAsync(int id);
        Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includeProperties);
        Task AddAsync(T entity);
        Task UpdateAsync(int id, T entity);
        Task DeleteAsync(int id);
    }
}
