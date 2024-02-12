using BackendDataAccessLayer;
using BackendDataAccessLayer.Entity;
using DataRepoCore;
using Microsoft.EntityFrameworkCore;

namespace DataRESTfulAPI {
    public class AppDbContext : DbContext, IAppDbContext{

        ///<inheritdoc/>
        public DbSet<EmployeeEntity> Employees { get; set; }

        ///<inheritdoc/>
        public DbSet<ArticleEntity> Article { get; set; }

        ///<inheritdoc/>
        public DbSet<DamageReportEntity> DamageReportEntities { get; set;}

        ///<inheritdoc/>
        public DbSet<PickingOrderEntity> PickingOrders { get; set;}

        ///<inheritdoc/>
        public DbSet<PickingOrderPositionEntity> PickingOrderPositions { get; set;}

        ///<inheritdoc/>
        public DbSet<StockPositionEntity> StockPositions { get; set;}

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Inspired by: https://learn.microsoft.com/en-us/ef/core/modeling/backing-field?tabs=data-annotations
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<EmployeeEntity>().Property("_passwordHash");
            modelBuilder.Entity<EmployeeEntity>().HasIndex(u => u.PersonnelNumber).IsUnique();
            modelBuilder.Entity<ArticleEntity>().HasIndex(a=>a.ArticleId).IsUnique();
        }
    }
}
