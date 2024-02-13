using BackendDataAccessLayer.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BackendDataAccessLayer.Repository {
    public abstract class BaseRepository<T> : IRepository<T> where T : class {
        protected DbContext _context;
        protected DbSet<T> _dbSet;

        public BaseRepository(DALDbContext context, DbSet<T> set) {
            this._context = context;
            _dbSet = set;
        }

        ///<inheritdoc/>
        public virtual async Task<bool> DeleteAsync(int id) {
            var element = await _dbSet.FindAsync(id);
            if (element is null)
                return false;
            _dbSet.Remove(element);
            return await _context.SaveChangesAsync() > 0;
        }

        ///<inheritdoc/>
        public virtual async Task<T?> GetElementByIDAsync(int id) {
            return await _dbSet.FindAsync(id);
        }
        ///<inheritdoc/>

        public virtual async Task<IEnumerable<T>> GetElementsAsync() {
            return await _dbSet.ToListAsync();
        }

        ///<inheritdoc/>
        public virtual async Task<bool> InsertAsync(T element) {
            await _dbSet.AddAsync(element);
            return await _context.SaveChangesAsync() > 0;
        }

        ///<inheritdoc/>
        public virtual async Task<bool> InsertRangeAsync(IEnumerable<T> elements) {
            foreach (var element in elements)
                await _dbSet.AddAsync(element);
            return await _context.SaveChangesAsync() > 0;
        }

        ///<inheritdoc/>
        public async Task<bool> UpdateAsync(T element) {
            _context.Update(element);
            return await _context.SaveChangesAsync() > 0;
        }

        ///<inheritdoc/>
        public virtual async Task<IEnumerable<T>> SelectAsync(Expression<Func<T, bool>> selector) {
            return await _dbSet.Where(selector).ToListAsync();
        }

        public virtual async Task<T?> FindAsync(Expression<Func<T, bool>> selector) {
            return await _dbSet.FirstAsync(selector);
        }

        ///<inheritdoc/>
        public virtual async Task<int> CountAsync(Expression<Func<T, bool>> selector) {
            return await _dbSet.Where(selector).CountAsync();
        }

        public virtual async Task<bool> ResetAsync() {
            _dbSet.RemoveRange(_dbSet);
            await _context.SaveChangesAsync();
            return (await CountAsync(a => true)) == 0;
        }

        public virtual void Dispose() {

        }
    }
}
