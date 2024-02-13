using BackendDataAccessLayer.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendDataAccessLayer {
    public abstract class DALDbContext : DbContext{
        public DALDbContext(DbContextOptions options) : base(options) {
        }

        /// <summary>
        /// All the employees employed.
        /// </summary>
        public DbSet<EmployeeEntity> Employees { get; set; }

        /// <summary>
        /// The articles in the warehouse.
        /// </summary>
        public DbSet<ArticleEntity> Article { get; set; }

        /// <summary>
        /// The damage reports filed.
        /// </summary>
        public DbSet<DamageReportEntity> DamageReportEntities { get; set; }

        /// <summary>
        /// The picking orders.
        /// </summary>
        public DbSet<PickingOrderEntity> PickingOrders { get; set; }

        /// <summary>
        /// The positions of picking orders.
        /// </summary>
        public DbSet<PickingOrderPositionEntity> PickingOrderPositions { get; set; }

        /// <summary>
        /// The stock positions in the warehouse.
        /// </summary>
        public DbSet<StockPositionEntity> StockPositions { get; set; }
    }
}
