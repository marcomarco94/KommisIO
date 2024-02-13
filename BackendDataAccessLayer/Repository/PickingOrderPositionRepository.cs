using BackendDataAccessLayer.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BackendDataAccessLayer.Repository {
    public class PickingOrderPositionRepository : BaseRepository<PickingOrderPositionEntity> {
        public PickingOrderPositionRepository(DALDbContext context) : base(context, context.PickingOrderPositions) {
        }

        /// <inheritdoc/>
        public override async Task<PickingOrderPositionEntity?> FindAsync(Expression<Func<PickingOrderPositionEntity, bool>> selector) {
            return await _dbSet.Include(d => d.Article).FirstAsync(selector);
        }

        /// <inheritdoc/>
        public override async Task<PickingOrderPositionEntity?> GetElementByIDAsync(int id) {
            return await _dbSet.Include(d => d.Article).FirstAsync(e => e.Id.Equals(id));
        }

        /// <inheritdoc/>
        public override async Task<IEnumerable<PickingOrderPositionEntity>> GetElementsAsync() {
            return await _dbSet.Include(d => d.Article).ToListAsync();
        }

        /// <inheritdoc/>
        public override async Task<IEnumerable<PickingOrderPositionEntity>> SelectAsync(Expression<Func<PickingOrderPositionEntity, bool>> selector) {
            return await _dbSet.Include(d => d.Article).Where(selector).ToListAsync();
        }
    }
}
