using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System;
using System.Text;
using yourOrder.APIs.Errors;
using yourOrder.APIs.Extensions;
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


            // handeling all error from model state to uniform error
            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    var errors = actionContext.ModelState
                        .Where(e => e.Value.Errors.Count > 0)
                        .SelectMany(e => e.Value.Errors)
                        .Select(e => e.ErrorMessage).ToArray();
                    var errorResponse = new ApiValidationErrorResponse {Errors = errors};
                    return new BadRequestObjectResult(errorResponse);
                };
            });

            

            builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
            builder.Services.AddApplicationServices(builder.Configuration);
            builder.Services.AddIdentityServices(builder.Configuration);
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

            // for check if database is healthy
            app.MapHealthChecks("/health", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });

            app.UseMiddleware<ExceptionMiddleware>(); // for only exceptions 500
            app.UseStatusCodePagesWithReExecute("/errors/{0}");// for other status code like 404,...
            app.UseStaticFiles(); //for images
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
