using System.Collections.Generic;
using System.Threading.Tasks;
using DatabaseSystem.Persistence.Enums;
using DatabaseSystem.Persistence.Models;

namespace DatabaseSystem.Services
{
    public interface IManagementService
    {
        #region Sync

        /// <summary>
        /// Create a new transaction
        /// </summary>
        /// <param name="operations">the transaction operation</param>
        /// <returns>the created transaction</returns>
        public Transaction CreateTransaction(IList<string> operations);

        /// <summary>
        /// Removes a transaction from the data base
        /// </summary>
        /// <param name="transaction">the transaction that will be removed</param>
        public void RemoveTransaction(Transaction transaction);

        /// <summary>
        /// Adds into the database a lock record
        /// </summary>
        /// <param name="transaction">the transaction that locked the data</param>
        /// <param name="lockType">the type of the lock (read or write)</param>
        /// <param name="lockedObject">the object that is locked</param>
        public void AcquireLock(Transaction transaction, LockType lockType, string lockedObject);

        /// <summary>
        /// Removes the lock record from database
        /// </summary>
        /// <param name="lock">the lock that will be removed</param>
        public void ReleaseLock(Lock @lock);

        /// <summary>
        /// Gets a transaction based on its id
        /// </summary>
        /// <param name="transactionId">the transaction id</param>
        /// <returns>the value of the transaction</returns>
        public Transaction FindTransactionById(int transactionId);

        /// <summary>
        /// This adds a record in wait for graph
        /// </summary>
        /// <param name="transactionThatNeedsLock">the transaction that wants to acquire a lock on a table</param>
        /// <param name="transactionThatHasLock">the transaction that has locks on that table</param>
        /// <param name="lockType">read or write</param>
        /// <param name="lockedObject">the object</param>
        public void AddTransactionDependency(Transaction transactionThatNeedsLock,
                                             Transaction transactionThatHasLock,
                                             LockType lockType,
                                             string lockedObject);
        /// <summary>
        /// Get all transactions
        /// </summary>
        /// <returns>a list of transactions</returns>
        public IList<Transaction> GetAllTransactions();

        /// <summary>
        /// Get all locks
        /// </summary>
        /// <returns>a list of locks</returns>
        public IList<Lock> GetAllLocks();

        /// <summary>
        /// Get all wait for graphs
        /// </summary>
        /// <returns>a list of all wait for graph</returns>
        public IList<WaitForGraph> GetAllWaitForGraphs();

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
