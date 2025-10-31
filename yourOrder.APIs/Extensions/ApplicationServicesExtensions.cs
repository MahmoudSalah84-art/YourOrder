using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using yourOrder.APIs.Helpers;
using yourOrder.Core.Entity.Identity;
using yourOrder.Core.Interfaces;
using yourOrder.Core.Services;
using yourOrder.Infrastructure.Data.Identity;
using yourOrder.Infrastructure.Repositories;
using yourOrder.Services;

namespace yourOrder.APIs.Extensions
{
    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration Configuration)
        {
            //editing here
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddAutoMapper(typeof(MappingProfiles));
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ICachingService, CachingService>();

            // add health checks
            var defaultConnection = Configuration.GetConnectionString("DefaultConnection");
            var identityConnection = Configuration.GetConnectionString("IdentityConnection");
            var redisConnection = Configuration.GetConnectionString("RedisConnection");
            services.AddHealthChecks()
                .AddSqlServer(defaultConnection!, name: "StoreDatabase")
                .AddSqlServer(identityConnection!, name: "IdentityDatabase")
                .AddRedis(redisConnection!, name: "RedisCache");

            // add CORS
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", policy =>
                {
                    policy.AllowAnyHeader()
                          .AllowAnyMethod()
                          .WithOrigins("https://localhost:3000");
                });
            });



            //waiting
            //services.Configure<ApiBehaviorOptions>(options =>
            //{
            //    options.InvalidModelStateResponseFactory = actionContext =>
            //    {
            //        var errors = actionContext.ModelState.Where(m => m.Value.Errors.Count > 0)
            //                                             .SelectMany(m => m.Value.Errors)
            //                                             .Select(e => e.ErrorMessage).ToArray();
            //        var ResponseMessage = new ValidationErrorResponse()
            //        {
            //            Errors = errors
            //        };
            //        return new BadRequestObjectResult(ResponseMessage);
            //    };
            //});

            return services;

        }
    }
}
