using System.Collections.Generic;
using System.Threading.Tasks;
using DatabaseSystem.Persistence.DatabaseContext;
using DatabaseSystem.Persistence.Enums;
using DatabaseSystem.Persistence.Models;
using DatabaseSystem.Persistence.Repository.Impl;

namespace DatabaseSystem.ConsoleApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var repo = new TransactionRepository(() => new TransactionalDbContext());
           
            var @lock = new LockRepository(() => new TransactionalDbContext());


            await @lock.AddAsync(new Lock
            {
                TransactionId = 27,
                LockType = LockType.Read,
                TableName = "a"
            });



            var t = await repo.FindByIdAsync(27, new List<string> {nameof(Transaction.Locks)});





        }
    }
}
