using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatabaseSystem.Persistence.DatabaseContext;
using DatabaseSystem.Persistence.Models;
using DatabaseSystem.Utility.ExtensionMethods;
using Microsoft.EntityFrameworkCore;

namespace DatabaseSystem.Persistence.Repository.Impl
{
    public class TransactionRepository : Abstract.AbstractRepository<Transaction, TransactionalDbContext>, ITransactionRepository
    {
        public TransactionRepository(Func<TransactionalDbContext> getContext) : base(getContext)
        {
        }
        protected override DbSet<Transaction> GetDatabaseSet(TransactionalDbContext context)
        {
            return context.Transactions;
        }

        public async Task<IList<Transaction>> FindTransactionsThatAreBlockingAsync(int transactionThatWantsToBeExecuted, Lock desiredLock)
        {
            //get all transactions from database
            var allTransactions = await GetAllAsync(new List<string>
            {
                nameof(Transaction.Locks),
                nameof(Transaction.Operations),
                nameof(Transaction.WaitForGraphsWantsLocks),
                nameof(Transaction.WaitForGraphsHasLocks)
            });

            await LockSemaphore.WaitAsync();
            try
            {
                //return only those transactions that are blocking the current transaction
                return await Task.Run(() =>
                {
                    return allTransactions
                        .Where(
                            activeTransaction =>
                                activeTransaction.TransactionId != transactionThatWantsToBeExecuted
                                && activeTransaction
                                    .Locks
                                    .Any(acquiredLock =>
                                        acquiredLock.LockType == desiredLock.LockType.GetOpposite() 
                                        && acquiredLock.TableName == desiredLock.TableName 
                                        && acquiredLock.Object == desiredLock.Object))
                        .ToList();
                });
            }
            finally
            {
                LockSemaphore.Release();
            }
        }
    }
}
