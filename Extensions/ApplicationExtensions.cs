
using System.Linq;
using Dreaming.Errors;
using Dreaming.Helper;
using Dreaming.Repositories;
using DreamMore.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Data.Extensions
{
    public static class ApplicationExtensions
    {
        public static IServiceCollection AppServices(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddSingleton<IResponseCache,ResponseCache>();
            services.AddScoped<IUnitOfWork,UnitOfWork>();
            services.AddScoped<IShoppingCart,ShoppingCartRepository>();
            services.AddScoped<IPaymentService,PaymentService>();
            services.AddScoped<IApplicationUserRepository,ApplicationUserRep>();
            services.AddAutoMapper(typeof(AutoMapProfiles));
            services.Configure<ApiBehaviorOptions>(opt =>
            {
                opt.InvalidModelStateResponseFactory = Context =>
                {
                    var errors = Context.ModelState.Values
                    .Where(x =>x.Errors.Count > 0)
                    .SelectMany(r => r.Errors)
                    .Select(m => m.ErrorMessage).ToArray();

                    var errorResults = new ApiValidation
                    {
                        Errors = errors 
                    };
                    return new BadRequestObjectResult(errorResults);
                };
            });
            return services;
        }
    }
}