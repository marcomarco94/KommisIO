using BackendDataAccessLayer.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BackendDataAccessLayer.Repository {
    public class DamageReportRepository : BaseRepository<DamageReportEntity> {
        public DamageReportRepository(DALDbContext context) : base(context, context.DamageReportEntities) {
        }

        /// <inheritdoc/>
        public override async Task<DamageReportEntity?> FindAsync(Expression<Func<DamageReportEntity, bool>> selector) {
            return await _dbSet.Include(d=>d.Article).Include(d=>d.Employee).FirstAsync(selector);
        }

        /// <inheritdoc/>
        public override async Task<DamageReportEntity?> GetElementByIDAsync(int id) {
            return await _dbSet.Include(d => d.Article).Include(d => d.Employee).FirstAsync(e => e.Id.Equals(id));
        }

        /// <inheritdoc/>
        public override async Task<IEnumerable<DamageReportEntity>> GetElementsAsync() {
            return await _dbSet.Include(d => d.Article).Include(d => d.Employee).ToListAsync();
        }

        /// <inheritdoc/>
        public override async Task<IEnumerable<DamageReportEntity>> WhereAsync(Expression<Func<DamageReportEntity, bool>> selector) {
            return await _dbSet.Include(d => d.Article).Include(d => d.Employee).Where(selector).ToListAsync();
        }
    }
}
