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
        public virtual async Task DeleteAsync(int id) {
            var element = await _dbSet.FindAsync(id);
            if (element is null)
                throw new ArgumentException(nameof(element));
            _dbSet.Remove(element);
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
        public virtual async Task InsertAsync(T element) {
            await _dbSet.AddAsync(element);
        }

        ///<inheritdoc/>
        public virtual async Task InsertRangeAsync(IEnumerable<T> elements) {
            foreach (var element in elements)
                await _dbSet.AddAsync(element);
        }

        ///<inheritdoc/>
        public async Task UpdateAsync(T element) {
            _context.Update(element);
            _context.Entry(element).State = EntityState.Modified;
        }

        ///<inheritdoc/>
        public virtual async Task<IEnumerable<T>> WhereAsync(Expression<Func<T, bool>> selector) {
            return await _dbSet.Where(selector).ToListAsync();
        }

        public virtual async Task<T?> FindAsync(Expression<Func<T, bool>> selector) {
            return await _dbSet.FirstAsync(selector);
        }

        ///<inheritdoc/>
        public virtual async Task<int> CountAsync(Expression<Func<T, bool>> selector) {
            return await _dbSet.Where(selector).CountAsync();
        }

        public virtual async Task ResetAsync() {
            _dbSet.RemoveRange(_dbSet);
        }

        public virtual void Dispose() {

        }
    }
}
