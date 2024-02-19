using BackendDataAccessLayer;
using BackendDataAccessLayer.Entity;
using BackendDataAccessLayer.Repository;
using DataRepoCore;
using DataRESTfulAPI;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Copied from https://learn.microsoft.com/de-at/azure/azure-sql/database/azure-sql-dotnet-entity-framework-core-quickstart?view=azuresql&tabs=visual-studio%2Cservice-connector%2Cportal
var connection = String.Empty;
if (builder.Environment.IsDevelopment()) {
    builder.Configuration.AddEnvironmentVariables().AddJsonFile("appsettings.Development.json");
    connection = builder.Configuration.GetConnectionString("AZURE_SQL_CONNECTIONSTRING");
}
else {
    connection = Environment.GetEnvironmentVariable("AZURE_SQL_CONNECTIONSTRING");
}

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connection));
//End of copy

builder.Services.AddScoped<DALDbContext, AppDbContext>();

builder.ConfigureServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

//Some test functions >>>>

//app.MapGet("/employee", async (IEmployeeRepository repo) => {
//    var res = await repo.GetElementsAsync();
//    return res.Select(e => e.MapToDataModel());
//});

//app.MapGet("/article", async (IArticleRepository repo) => {
//    return (await repo.GetElementsAsync()).Select(e => e.MapToDataModel());
//});

//app.MapGet("/damagereport", async (IRepository<DamageReportEntity> repo) => {
//    return (await repo.GetElementsAsync()).Select(e => e.MapToDataModel());
//});

//app.MapGet("/stockposition", async (IRepository<StockPositionEntity> repo) => {
//    return (await repo.GetElementsAsync()).Select(e => e.MapToDataModel());
//});

//app.MapGet("/pickingorder", async (IRepository<PickingOrderEntity> repo) => {
//    return (await repo.GetElementsAsync()).Select(e => e.MapToDataModel());
//});


//app.MapGet("/reset", async (IDemoDataBuilder builder) => {
//    await builder.BuildDemoDataAsync();
//    return true;
//});

app.Run();
