using BackendDataAccessLayer.Entity;
using BackendDataAccessLayer.Repository;
using BackendDataAccessLayer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Microsoft.Identity.Web;

namespace DataRESTfulAPI {
    public static class ExtensionMethods {
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

            //Inspired by Jesse (2019): https://stackoverflow.com/questions/58179180/jwt-authentication-and-swagger-with-net-core-3-0
            builder.Services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo {
                    Title = "KommissIO",
                    Version = "v1"
                });
                c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme() {
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "JWT Bearer: ",
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                  });
            });
        }

        //Inspired by Dev Empower (2023): https://www.youtube.com/watch?v=KRVjIgr-WOU
        public static void ConfigureAuthentication(this WebApplicationBuilder builder) {
            builder.Services.AddAuthentication(options => {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options => {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters() {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
                    ValidAudience = builder.Configuration["JWT:ValidAudience"],

#if DEBUG
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"] ??
                    throw new NullReferenceException("Unable to find configured secret.")))
#else
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("KommissIOJWTSecret") ??
                    throw new NullReferenceException("Unable to find configured secret.")))
#endif
                };
            });
        }
    }
}
