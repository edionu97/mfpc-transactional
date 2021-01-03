using System.Collections.Generic;
using System.Threading.Tasks;
using DatabaseSystem.Persistence.Models;
using DatabaseSystem.Utility.Enums;

namespace DatabaseSystem.Services.Management
{
    public interface IManagementService
    {
        public Task<Transaction> CreateTransactionAsync(IList<Operation> operations);

        public Task RemoveTransactionAsync(Transaction transaction);

        public Task AcquireLockAsync(Transaction transaction, LockType lockType, string lockedObject);

        public Task ReleaseLockAsync(Lock @lock);

        public Task<Transaction> FindTransactionByIdAsync(int transactionId);

        public Task<IList<Transaction>> FindTransactionsThatAreBlockingAsync(
            int transactionThatWantsToBeExecuted,
            Lock desiredLock);

        public Task<IList<Transaction>> GetAllTransactionsAsync();

        public Task<IList<Lock>> GetAllLocksAsync();
    }
}
