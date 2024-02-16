using BackendDataAccessLayer.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BackendDataAccessLayer.Repository {
    public class StockPositionRepository : BaseRepository<StockPositionEntity> {
        public StockPositionRepository(DALDbContext context) : base(context, context.StockPositions) {
        }

        /// <inheritdoc/>
        public override async Task<StockPositionEntity?> FindAsync(Expression<Func<StockPositionEntity, bool>> selector) {
            return await _dbSet.Include(d => d.Article).FirstAsync(selector);
        }

        /// <inheritdoc/>
        public override async Task<StockPositionEntity?> GetElementByIDAsync(int id) {
            return await _dbSet.Include(d => d.Article).FirstAsync(e => e.Id.Equals(id));
        }

        /// <inheritdoc/>
        public override async Task<IEnumerable<StockPositionEntity>> GetElementsAsync() {
            return await _dbSet.Include(d => d.Article).ToListAsync();
        }

        /// <inheritdoc/>
        public override async Task<IEnumerable<StockPositionEntity>> WhereAsync(Expression<Func<StockPositionEntity, bool>> selector) {
            return await _dbSet.Include(d => d.Article).Where(selector).ToListAsync();
        }
    }
}
