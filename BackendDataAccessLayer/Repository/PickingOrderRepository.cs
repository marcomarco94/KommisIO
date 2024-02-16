using BackendDataAccessLayer.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BackendDataAccessLayer.Repository {
    public class PickingOrderRepository : BaseRepository<PickingOrderEntity> {
        public PickingOrderRepository(DALDbContext context) : base(context, context.PickingOrders) {
        }

        /// <inheritdoc/>
        public override async Task<PickingOrderEntity?> FindAsync(Expression<Func<PickingOrderEntity, bool>> selector) {
            return await _dbSet.Include(d => d.Positions)!.ThenInclude(e => e.Article).Include(d => d.Employee).FirstAsync(selector);
        }

        /// <inheritdoc/>
        public override async Task<PickingOrderEntity?> GetElementByIDAsync(int id) {
            return await _dbSet.Include(d => d.Positions)!.ThenInclude(e => e.Article).Include(d => d.Employee).FirstAsync(e => e.Id.Equals(id));
        }

        /// <inheritdoc/>
        public override async Task<IEnumerable<PickingOrderEntity>> GetElementsAsync() {
            return await _dbSet.Include(d => d.Positions)!.ThenInclude(e => e.Article).Include(d => d.Employee).ToListAsync();
        }

        /// <inheritdoc/>
        public override async Task<IEnumerable<PickingOrderEntity>> WhereAsync(Expression<Func<PickingOrderEntity, bool>> selector) {
            return await _dbSet.Include(d => d.Positions)!.ThenInclude(e => e.Article).Include(d => d.Employee).Where(selector).ToListAsync();
        }
    }
}
