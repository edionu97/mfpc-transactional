using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DatabaseSystem.Persistence.DatabaseContext.ContextFactory;
using DatabaseSystem.Persistence.Enums;
using DatabaseSystem.Persistence.Models;
using DatabaseSystem.Persistence.Repository.Impl;
using DatabaseSystem.Transactional.Graph.Element;

namespace DatabaseSystem.ConsoleApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var repo = new TransactionRepository(
                () => new TransactionalDbContextFactory().CreateDbContext(null));

           
            var @lock = new LockRepository(() => new TransactionalDbContextFactory().CreateDbContext(null));


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
