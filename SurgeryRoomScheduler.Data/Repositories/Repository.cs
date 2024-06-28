using Microsoft.EntityFrameworkCore;
using SurgeryRoomScheduler.Data.Context;
using SurgeryRoomScheduler.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SurgeryRoomScheduler.Data.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly AppDbContext Context;
        protected DbSet<T> entities;

        public Repository(AppDbContext context)
        {
            Context = context;
            entities = Context.Set<T>();
        }

        public async Task AddAsync(T entity)
        {
            try
            {
                await entities.AddAsync(entity);
                await Context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await entities.ToListAsync();
        }

        public async Task<IEnumerable<T?>> GetAllAsync(Expression<Func<T, bool>> filter)
        {
            return await entities.Where(filter).ToListAsync();
        }

        public async Task<T?> GetAsync(int id)
        {
            return await entities.FindAsync(id);
        }

        public async Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> filter)
        {
            return await entities.SingleOrDefaultAsync(filter);
        }

        public void Remove(T entity)
        {
            entities.Remove(entity);
        }

        public virtual async Task UpdateAsync(T entity)
        {
            using var transaction = await Context.Database.BeginTransactionAsync();

            try
            {
                entities.Update(entity);
                await Context.SaveChangesAsync();
                await transaction.CommitAsync();

            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<int> GetCountAsync(Expression<Func<T, bool>> filter)
        {
            return await entities.Where(filter).CountAsync();
        }

        public async Task<bool> IsExist(Expression<Func<T, bool>> filter)
        {
            return await entities.AnyAsync(filter);
        }

        public async Task AddRangeAsync(List<T> entity)
        {

            try
            {
                await entities.AddRangeAsync(entity);
                await Context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> filter)
        {
            return await entities.FirstOrDefaultAsync(filter);
        }
        public async Task<T?> LastOrDefaultAsync(Expression<Func<T, bool>> filter)
        {
            return await entities.LastOrDefaultAsync(filter);
        }
    }
}
