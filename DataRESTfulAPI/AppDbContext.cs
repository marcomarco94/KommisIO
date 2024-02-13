using BackendDataAccessLayer;
using BackendDataAccessLayer.Entity;
using DataRepoCore;
using Microsoft.EntityFrameworkCore;

namespace DataRESTfulAPI {
    public class AppDbContext : DALDbContext{
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {
        }

        // Inspired by: https://learn.microsoft.com/en-us/ef/core/modeling/backing-field?tabs=data-annotations
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<EmployeeEntity>().Property("_passwordHash");
            modelBuilder.Entity<EmployeeEntity>().HasIndex(u => u.PersonnelNumber).IsUnique();
            modelBuilder.Entity<ArticleEntity>().HasIndex(a=>a.ArticleId).IsUnique();
        }
    }
}
