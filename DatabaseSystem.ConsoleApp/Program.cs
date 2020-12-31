using System.Threading.Tasks;
using DatabaseSystem.ConsoleApp.Config;
using DatabaseSystem.Persistence.Enums;
using DatabaseSystem.Persistence.Models;
using DatabaseSystem.Persistence.Repository;
using DatabaseSystem.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DatabaseSystem.ConsoleApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {

            var host = Bootstrapper.Load();

            using var serviceRepo = host.Services.CreateScope();

            var managementService = serviceRepo.ServiceProvider.GetRequiredService<IManagementService>();

            //var repo = new TransactionRepository(() => new TransactionalDbContext());

            //var @lock = new LockRepository(() => new TransactionalDbContext());


            //await @lock.AddAsync(new Lock
            //{
            //    TransactionId = 27,
            //    LockType = LockType.Read,
            //    TableName = "a"
            //});



            //var t = await repo.FindByIdAsync(27, new List<string> {nameof(Transaction.Locks)});





        }
    }
}
