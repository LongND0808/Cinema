using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Core.InterfaceRepository
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task<TEntity?> GetByIdAsync(Guid id);
        Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? filter = null);
        Task<TEntity?> GetOneAsync(Expression<Func<TEntity, bool>>? filter = null);
        Task<IEnumerable<TEntity>> GetAllAsyncUntracked(Expression<Func<TEntity, bool>>? filter = null);
        Task<TEntity?> GetOneAsyncUntracked(Expression<Func<TEntity, bool>>? filter = null);
        Task<TResult[]?> GetAllAsyncUntracked<TResult>(Expression<Func<TEntity, bool>>? filter = null,
            Expression<Func<TEntity, TResult>>? selector = null);
        Task<TResult?> GetOneAsyncUntracked<TResult>(Expression<Func<TEntity, bool>>? filter = null, 
            Expression<Func<TEntity, TResult>>? selector = null);
        Task AddAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task DeleteAsync(TEntity entity);
        Task<TEntity?> GetUserByUserNameAsync(string username);
        Task<IEnumerable<Guid>?> GetRoleIdsByUserID(Guid userId);
    }
}
