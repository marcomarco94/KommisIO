using BackendDataAccessLayer;
using BackendDataAccessLayer.Entity;
using BackendDataAccessLayer.Repository;
using DataRepoCore;
using DataRESTfulAPI;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//Copied from https://learn.microsoft.com/de-at/azure/azure-sql/database/azure-sql-dotnet-entity-framework-core-quickstart?view=azuresql&tabs=visual-studio%2Cservice-connector%2Cportal
var connection = String.Empty;
builder.Configuration.AddEnvironmentVariables();
if (builder.Environment.IsDevelopment()) {
    builder.Configuration.AddEnvironmentVariables().AddJsonFile("appsettings.Development.json");
}

connection = builder.Configuration.GetConnectionString("AZURE_SQL_CONNECTIONSTRING");

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connection, builder => {
    builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
}));
//End of copy

builder.Services.AddScoped<DALDbContext, AppDbContext>();

builder.ConfigureServices();
builder.ConfigureAuthentication();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
