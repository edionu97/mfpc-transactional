using System.Threading.Tasks;
using DatabaseSystem.ConsoleApp.Config;
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

            ////var transaction1 = await managementService.FindTransactionByIdAsync(31);
            //var transaction2 = await managementService.CreateTransactionAsync(new List<string>
            //{
            //    "select",
            //    "delete"
            //});
            //var transaction3 = await managementService.FindTransactionByIdAsync(2);

            //await managementService.AcquireLockAsync(transaction2, LockType.Read, "a");
            //await managementService.AcquireLockAsync(transaction2, LockType.Write, "a");

            //await managementService.AddTransactionDependencyAsync(transaction3, transaction2, LockType.Write, "a");

           

            foreach (var el in await managementService.GetAllTransactionsAsync())
            {
                await managementService.RemoveTransactionAsync(el);
            }

           

            //await managementService.RemoveTransactionAsync(transaction2);

            //await managementService.AddTransactionDependencyAsync(transaction2, transaction3, LockType.Write, "b");



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
