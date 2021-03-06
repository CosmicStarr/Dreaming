
using System.IO;
using Data;
using Data.Extensions;
using Dreaming.Data;
using Dreaming.Extensions;
using Dreaming.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Models;
using StackExchange.Redis;

namespace Dreaming
{
    public class Startup
    {
        private readonly IConfiguration configuration;
        public Startup(IConfiguration Configuration)
        {
            configuration = Configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            
            services.AddIdentities(configuration);
            services.AddIdentity<ApplicationUser,IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
            services.AddDbContext<ApplicationDbContext>(options =>{
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddSingleton<IConnectionMultiplexer>(r =>{
                var config = ConfigurationOptions.Parse(configuration.GetConnectionString("Redis"),true);
                return ConnectionMultiplexer.Connect(config);
            });
       
            services.AddScoped<IOrderRepository,OrderRepositiory>();
            services.AddControllers();
            services.AppServices(configuration);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Dreaming", Version = "v1" });
                 var SecuritySchema = new OpenApiSecurityScheme
                {
                    Description = "JWT Bearer Security",
                    Name = "Json Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    Reference = new OpenApiReference
                    {
                        Id = "Bearer",
                        Type = ReferenceType.SecurityScheme,
                    }
                };
                c.AddSecurityDefinition("Bearer", SecuritySchema);
                var SecurityRequirements = new OpenApiSecurityRequirement
                {
                    {
                        SecuritySchema, new[]
                        {
                            "Bearer"
                        }
                    }
                };
                c.AddSecurityRequirement(SecurityRequirements);
            });
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy => 
                {
                    policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
                });
           
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Dreaming v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(),"Content")
                ),RequestPath = "/content"
            });
            app.UseCors();
            app.UseAuthentication();
            app.UseAuthorization();
    
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapFallbackToController("Index","FallBack");
            });
        }
    }
}
