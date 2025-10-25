using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
//using NETCore.MailKit.Core;
using StackExchange.Redis;
using System;
using System.Text;
using yourOrder.APIs.Helpers;
using yourOrder.APIs.Middleware;
using yourOrder.Core.Entity.Identity;
using yourOrder.Core.Interfaces;
using yourOrder.Core.Services;
using yourOrder.Infrastructure.Data;
using yourOrder.Infrastructure.Data.Identity;
using yourOrder.Infrastructure.Repositories;
using yourOrder.Services;
namespace yourOrder.APIs
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            #region Create a builder and configure services
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllers();
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            builder.Services.AddDbContext<AppIdentityDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection")));
            builder.Services.AddSingleton<IConnectionMultiplexer>(c => 
            {
                var connection = builder.Configuration.GetConnectionString("RedisConnection");
                return ConnectionMultiplexer.Connect(connection);
            });
            builder.Services.AddScoped<IBasketRepository, BasketRepository>();
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddAutoMapper(typeof(MappingProfiles));
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<AppIdentityDbContext>()
                .AddDefaultTokenProviders();
            builder.Services.AddAuthorization();
            
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; // how system identify user from token
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // how system deal with user who is unauthorized
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    ClockSkew = TimeSpan.FromMinutes(1),
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                };
            });
            #endregion

            var app = builder.Build();

            #region Configure the HTTP request pipeline.
            //Seeding Data
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var loggerFactory = services.GetRequiredService<ILoggerFactory>(); // Get logger factory for logging
            try
            {
                var context = services.GetRequiredService<AppDbContext>(); //create instance from appdbcontext and add required for throw exeption if null not just null (connect to database)
                await context.Database.MigrateAsync(); // Apply pending migrations
                await AppDbContextSeed.SeedAsync(context, loggerFactory); // Seed the data
                var userManager = services.GetRequiredService<UserManager<AppUser>>(); //create instance from usermanager and add required for throw exeption if null not just null (connect to database)
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>(); //create instance from rolemanager and add required for throw exeption if null not just null (connect to database)
                await AppIdentityDbContextSeeding.SeedUsersAsync(userManager , roleManager , loggerFactory);
            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger<Program>();
                logger.LogError(ex, "An error occurred during migration");
            }
            // End Seeding Data
            app.UseMiddleware<ExceptionMiddleware>(); // for only exceptions 500
            app.UseStatusCodePagesWithReExecute("/errors/{0}");// for other status code like 404
            app.UseHttpsRedirection();
            app.UseRouting(); 
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            #endregion
            app.Run();
        }
    }
}
