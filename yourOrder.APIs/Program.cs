
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System;
using yourOrder.APIs.Helpers;
using yourOrder.APIs.Middleware;
using yourOrder.Core.Interfaces;
using yourOrder.Infrastructure.Data;
using yourOrder.Infrastructure.Repositories;

namespace yourOrder.APIs
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            // Add DbContext with SQL Server
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            // Add Redis Connection
            builder.Services.AddSingleton<IConnectionMultiplexer>(c => 
            {
                var connection = builder.Configuration.GetConnectionString("RedisConnection");
                return ConnectionMultiplexer.Connect(connection);
            });

            builder.Services.AddScoped<IBasketRepository, BasketRepository>();
            // ...


            // Add Generic Repository
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            // Add AutoMapper
            builder.Services.AddAutoMapper(typeof(MappingProfiles));

            







            var app = builder.Build();
            // Configure the HTTP request pipeline.


            


            //Seeding Data
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var loggerFactory = services.GetRequiredService<ILoggerFactory>(); // Get logger factory for logging
            try
            {
                var context = services.GetRequiredService<AppDbContext>(); //create instance from appdbcontext and add required for throw exeption if null not just null (connect to database)
                await context.Database.MigrateAsync(); // Apply pending migrations
                await AppDbContextSeed.SeedAsync(context, loggerFactory); // Seed the data
            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger<Program>();
                logger.LogError(ex, "An error occurred during migration");

            }
            // End Seeding Data


            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi(); 
            }
            app.UseMiddleware<ExceptionMiddleware>(); // catch exceptions globally
            app.UseStatusCodePagesWithReExecute("/errors/{0}");// redirect to errors controller



            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
