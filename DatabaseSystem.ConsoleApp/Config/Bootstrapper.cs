using DatabaseSystem.Persistence.DatabaseContext;
using DatabaseSystem.Persistence.Models;
using DatabaseSystem.Persistence.Repository;
using DatabaseSystem.Persistence.Repository.Impl;
using DatabaseSystem.Services;
using DatabaseSystem.Services.Impl;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
                    //database
                    services.AddSingleton<IRepository<Transaction>, TransactionRepository>(
                        x => new TransactionRepository(() => new TransactionalDbContext()));

                    services.AddSingleton<IRepository<Lock>, LockRepository>(
                        x => new LockRepository(() => new TransactionalDbContext()));

                    services.AddSingleton<IRepository<WaitForGraph>, DependenciesRepository>(
                        x => new DependenciesRepository(() => new TransactionalDbContext()));

                    //services
                    services.AddSingleton<IManagementService, ManagementService>();

                })
                .Build();
        }
    }
}
