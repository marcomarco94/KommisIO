using BackendDataAccessLayer.Entity;
using BackendDataAccessLayer.Repository;
using BackendDataAccessLayer;
using Microsoft.AspNetCore.Identity;

namespace DataRESTfulAPI {
    public static class RegisterServices {
        public static void ConfigureServices(this WebApplicationBuilder builder) {
            builder.Services.AddSingleton<IPasswordHasher<EmployeeEntity>, EmployeePasswordHasher>();

            //Setup Repositories
            builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            builder.Services.AddScoped<IArticleRepository, ArticleRepository>();
            builder.Services.AddScoped<IRepository<DamageReportEntity>, DamageReportRepository>();
            builder.Services.AddScoped<IRepository<PickingOrderEntity>, PickingOrderRepository>();
            builder.Services.AddScoped<IRepository<StockPositionEntity>, StockPositionRepository>();
            builder.Services.AddScoped<IRepository<PickingOrderPositionEntity>, PickingOrderPositionRepository>();

            builder.Services.AddScoped<IDemoDataBuilder, DemoDataBuilder>();
        }
    }
}
