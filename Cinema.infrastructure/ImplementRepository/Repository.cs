using Cinema.Core.InterfaceRepository;
using Cinema.Domain.Entities;
using Cinema.Infrastructure.DataContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<TEntity?> GetByIdAsync(int id)
        {
            return await _context.Set<TEntity>().FindAsync(id);
        }

        public async Task<IEnumerable<TEntity>?> GetAllAsync()
        {
            return await _context.Set<TEntity>().ToListAsync();
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

        public async Task<IEnumerable<int>?> GetRoleIdsByUserID(int userId)
        {
            if (typeof(TEntity) == typeof(UserRole))
            {
                var res = await _context.Set<UserRole>()
                    .Where(u => u.UserID == userId).Select(x => x.Id).ToListAsync();

                return res;
            }

            throw new InvalidOperationException("Invalid entity type");
        }
    }
}
