using BackendDataAccessLayer.Entity;
using BackendDataAccessLayer.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendDataAccessLayer {
    //Inspired by Tom Dykstra (2022) at https://learn.microsoft.com/en-us/aspnet/mvc/overview/older-versions/getting-started-with-ef-5-using-mvc-4/implementing-the-repository-and-unit-of-work-patterns-in-an-asp-net-mvc-application
    public interface IKommissIOUnitOfWork : IDisposable{
        public IRepository<PickingOrderEntity> PickingOrderRepository { get; init; }
        public IRepository<DamageReportEntity> DamageReportRepository { get; init; }
        public IRepository<StockPositionEntity> StockPositionRepository { get; init; }
        public IArticleRepository ArticleRepository { get; init; }
        public IEmployeeRepository EmployeeRepository { get; init; }
        public IRepository<PickingOrderPositionEntity> PickingOrderPositionRepository { get; init; }
        
        /// <summary>
        /// Commit the work done.
        /// </summary>
        public void Commit();

        /// <summary>
        /// Commit the work done.
        /// </summary>
        /// <returns>Retruns the task.</returns>
        public Task CommitAsync();
    }
}
