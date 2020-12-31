using System.Collections.Generic;
using System.Threading.Tasks;
using DatabaseSystem.Persistence.Enums;
using DatabaseSystem.Persistence.Models;

namespace DatabaseSystem.Services
{
    public interface IManagementService
    {
        #region Sync

        public Transaction CreateTransaction(IList<string> operations);
        public void RemoveTransaction(Transaction transaction);
        public void AcquireLock(Transaction transaction, LockType lockType, string lockedObject);
        public void ReleaseLock(Lock @lock);
        public Transaction FindTransactionById(int transactionId);

        public void AddTransactionDependency(Transaction transactionThatNeedsLock,
                                             Transaction transactionThatHasLock,
                                             LockType lockType,
                                             string lockedObject);

        #endregion


        #region Async

        public Task<Transaction> CreateTransactionAsync(IList<string> operations);
        public Task RemoveTransactionAsync(Transaction transaction);
        public Task AcquireLockAsync(Transaction transaction, LockType lockType, string lockedObject);
        public Task ReleaseLockAsync(Lock @lock);
        public Task<Transaction> FindTransactionByIdAsync(int transactionId);
        public Task AddTransactionDependencyAsync(Transaction transactionThatNeedsLock,
                                                  Transaction transactionThatHasLock,
                                                  LockType lockType,
                                                  string lockedObject);
        public Task<IList<Transaction>> GetAllTransactionsAsync();
        public Task<IList<Lock>> GetAllLocksAsync();
        public Task<IList<WaitForGraph>> GetAllWaitForGraphsAsync();

        #endregion
    }
}
