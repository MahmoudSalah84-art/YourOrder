
using Microsoft.EntityFrameworkCore;
using System;
using yourOrder.APIs.Helpers;
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

            // Add Generic Repository
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            // Add AutoMapper
            builder.Services.AddAutoMapper(typeof(MappingProfiles));










            var app = builder.Build();

            // Configure the HTTP request pipeline.

              
            // --- بداية الإضافة (Seeding Data) ---
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var loggerFactory = services.GetRequiredService<ILoggerFactory>();
            try
            {
                var context = services.GetRequiredService<AppDbContext>();
                await context.Database.MigrateAsync(); // Apply pending migrations
                await AppDbContextSeed.SeedAsync(context, loggerFactory); // Seed the data
            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger<Program>();
                logger.LogError(ex, "An error occurred during migration");
            }
            // --- نهاية الإضافة ---


            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
