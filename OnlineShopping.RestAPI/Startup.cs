using DatabaseSystem.Persistence.DatabaseContext;
using DatabaseSystem.Persistence.Models;
using DatabaseSystem.Persistence.Repository;
using DatabaseSystem.Persistence.Repository.Impl;
using DatabaseSystem.Services.Management;
using DatabaseSystem.Services.Management.Impl;
using DatabaseSystem.Services.Scheduling;
using DatabaseSystem.Services.Scheduling.Impl;
using DatabaseSystem.Services.SqlExecutor;
using DatabaseSystem.Services.SqlExecutor.Impl;
using DatabaseSystem.Transactional.Graph;
using DatabaseSystem.Transactional.Graph.Impl;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OnlineShopping.Services;
using OnlineShopping.Services.Impl;

namespace OnlineShopping.RestAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowOrigin", 
                      builder =>
                      {
                           builder.WithOrigins(
                                "http://localhost:59356",
                                "http://localhost:3000")
                            .AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                      });
            });

            services.AddControllers();
           
            //add the swagger 
            services.AddSwaggerGen();

            //utility
            services.AddSingleton<IGraph, ConcurrencyGraph>();

            //database
            services.AddSingleton<ITransactionRepository, TransactionRepository>(
                x => new TransactionRepository(() => new TransactionalDbContext()));

            services.AddSingleton<IRepository<Lock>, LockRepository>(
                x => new LockRepository(() => new TransactionalDbContext()));

            //services
            services.AddSingleton<ISqlExecutorService, SqlExecutorService>(
                x => new SqlExecutorService(
                    "Data Source=DESKTOP-VQ4KD11;Initial Catalog=Shop;Integrated Security=True"));

            services.AddSingleton<IManagementService, ManagementService>();

            services.AddSingleton<ISchedulingService, SchedulingService>();

            //application service 
            services.AddSingleton<IShoppingService, ShoppingService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //swagger is on http://localhost:59356/swagger/index.html
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "RestAPI data");
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseCors();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
