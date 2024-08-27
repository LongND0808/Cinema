using Cinema.Core.InterfaceRepository;
using Cinema.Domain.Entities;
using Cinema.Infrastructure.DataContext;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Cinema.Infrastructure.ImplementRepository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly ApplicationDbContext _context;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<TEntity?> GetByIdAsync(Guid id)
        {
            return await _context.Set<TEntity>().FindAsync(id);
        }

        public async Task AddAsync(TEntity entity)
        {
            await _context.Set<TEntity>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TEntity entity)
        {
            _context.Set<TEntity>().Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<TEntity?> GetUserByUserNameAsync(string username)
        {
            if (typeof(TEntity) == typeof(User))
            {
                var res = await _context.Set<User>()
                    .FirstOrDefaultAsync(u => u.UserName == username) as TEntity;

                return res;
            }

            throw new InvalidOperationException("Invalid entity type");
        }

        public async Task<IEnumerable<Guid>?> GetRoleIdsByUserID(Guid userId)
        {
            if (typeof(TEntity) == typeof(UserRole))
            {
                var res = await _context.Set<UserRole>()
                    .Where(u => u.UserId == userId).Select(x => x.RoleId).ToListAsync();

                return res;
            }

            throw new InvalidOperationException("Invalid entity type");
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? filter = null)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.ToListAsync();
        }

        public async Task<TEntity?> GetOneAsync(Expression<Func<TEntity, bool>>? filter = null)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.FirstOrDefaultAsync();
        }
        public async Task<IEnumerable<TEntity>> GetAllAsyncUntracked(Expression<Func<TEntity, bool>>? filter = null)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>().AsNoTracking();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.ToListAsync();
        }

        public async Task<TEntity?> GetOneAsyncUntracked(Expression<Func<TEntity, bool>>? filter = null)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>().AsNoTracking();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task<TResult[]?> GetAllAsyncUntracked<TResult>(
            Expression<Func<TEntity, bool>>? filter = null,
            Expression<Func<TEntity, TResult>>? selector = null)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>().AsNoTracking();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (selector != null)
            {
                return await query.Select(selector).ToArrayAsync();
            }
            else
            {
                var results = await query.ToArrayAsync();
                return results.Select(x => (TResult)(object)x).ToArray();
            }
        }

        public async Task<TResult?> GetOneAsyncUntracked<TResult>(
            Expression<Func<TEntity, bool>>? filter = null,
            Expression<Func<TEntity, TResult>>? selector = null)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>().AsNoTracking();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (selector != null)
            {
                return await query.Select(selector).FirstOrDefaultAsync();
            }
            else
            {
                var result = await query.FirstOrDefaultAsync();
                return (TResult?) (object?) result; 
            }
        }
    }
}
