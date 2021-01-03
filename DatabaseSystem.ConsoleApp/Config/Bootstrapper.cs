using DatabaseSystem.Persistence.DatabaseContext;
using DatabaseSystem.Persistence.Models;
using DatabaseSystem.Persistence.Repository;
using DatabaseSystem.Persistence.Repository.Impl;
using DatabaseSystem.Services.Management;
using DatabaseSystem.Services.Scheduling;
using DatabaseSystem.Services.Scheduling.Impl;
using DatabaseSystem.Services.SqlExecutor;
using DatabaseSystem.Services.SqlExecutor.Impl;
using DatabaseSystem.Transactional.Graph;
using DatabaseSystem.Transactional.Graph.Impl;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ManagementService = DatabaseSystem.Services.Management.Impl.ManagementService;

namespace DatabaseSystem.ConsoleApp.Config
{
    public static class Bootstrapper
    {
        public static IHost Load()
        {
            return Host
                .CreateDefaultBuilder()
                .ConfigureAppConfiguration(config =>
                {
                    config.AddJsonFile("appsettings.json", optional: true);
                })
                .ConfigureServices((context, services) =>
                {
                    //utility
                    services.AddSingleton<IGraph, ConcurrencyGraph>();

                    //database
                    services.AddSingleton<ITransactionRepository, TransactionRepository>(
                        x => new TransactionRepository(() => new TransactionalDbContext()));

                    services.AddSingleton<IRepository<Lock>, LockRepository>(
                        x => new LockRepository(() => new TransactionalDbContext()));

                    //services
                    services.AddSingleton<ISqlExecutorService, SqlExecutorService>(
                        x => new SqlExecutorService("Connection String"));

                    services.AddSingleton<IManagementService, ManagementService>();

                    services.AddSingleton<ISchedulingService, SchedulingService>();

                })
                .Build();
        }
    }
}
