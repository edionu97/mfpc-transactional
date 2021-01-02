using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DatabaseSystem.Persistence.Models;
using DatabaseSystem.Persistence.Repository;
using DatabaseSystem.Utility.Enums;

namespace DatabaseSystem.Services.Management.Impl
{
    public class ManagementService : IManagementService
    {
        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);

        private readonly ITransactionRepository _transactionRepository;
        private readonly IRepository<Lock> _lockRepository;
        private readonly IRepository<WaitForGraph> _dependencyRepository;


        public ManagementService(IRepository<WaitForGraph> dependencyRepository,
                                 IRepository<Lock> lockRepository,
                                 ITransactionRepository transactionRepository)
        {
            _dependencyRepository = dependencyRepository;
            _lockRepository = lockRepository;
            _transactionRepository = transactionRepository;
        }

        public async Task<Transaction> CreateTransactionAsync(IList<Operation> operations)
        {
            //await _semaphoreSlim.WaitAsync();
            //try
            //{
            //create the transaction
            var transaction = new Transaction();
            foreach (var operation in operations)
            {
                transaction.Operations.Add(operation);
            }

            //add the transaction and return the result
            await _transactionRepository.AddAsync(transaction);
            return transaction;
            //}
            //finally
            //{
            //    _semaphoreSlim.Release();
            //}
        }

        public async Task RemoveTransactionAsync(Transaction transaction)
        {
            //await _semaphoreSlim.WaitAsync();
            //try
            //{
            foreach (var waitForGraph in
                transaction
                    .WaitForGraphsWantsLocks
                    .Concat(transaction.WaitForGraphsHasLocks).ToList())
            {
                await _dependencyRepository.DeleteAsync(waitForGraph);
            }

            await _transactionRepository.DeleteAsync(transaction);
            //}
            //finally
            //{
            //    _semaphoreSlim.Release();
            //}
        }

        public async Task AcquireLockAsync(Transaction transaction, LockType lockType, string lockedObject)
        {
            //await _semaphoreSlim.WaitAsync();
            //try
            //{
            //create the lock
            var @lock = new Lock
            {
                Object = lockedObject,
                TableName = lockedObject,
                LockType = lockType,
                TransactionId = transaction.TransactionId
            };

            //add the lock
            await _lockRepository.AddAsync(@lock);
            //}
            //finally
            //{
            //    _semaphoreSlim.Release();
            //}
        }

        public async Task ReleaseLockAsync(Lock @lock)
        {
            //await _semaphoreSlim.WaitAsync();
            //try
            //{
            await _lockRepository.DeleteAsync(@lock);
            //}
            //finally
            //{
            //    _semaphoreSlim.Release();
            //}
        }

        public async Task<Transaction> FindTransactionByIdAsync(int transactionId)
        {
            //await _semaphoreSlim.WaitAsync();
            //try
            //{
            return await _transactionRepository.FindByIdAsync(transactionId, new List<string>
                {
                    nameof(Transaction.Locks),
                    nameof(Transaction.Operations),
                    nameof(Transaction.WaitForGraphsWantsLocks),
                    nameof(Transaction.WaitForGraphsHasLocks)
                });
            //}
            //finally
            //{
            //    _semaphoreSlim.Release();
            //}
        }

        public async Task<IList<Transaction>> FindTransactionsThatAreBlockingAsync(int transactionThatWantsToBeExecuted, Lock desiredLock)
        {
            //await _semaphoreSlim.WaitAsync();
            //try
            //{
            return
                await _transactionRepository
                    .FindTransactionsThatAreBlockingAsync(transactionThatWantsToBeExecuted, desiredLock);
            //}
            //finally
            //{
            //    _semaphoreSlim.Release();
            //}
        }

        public async Task<WaitForGraph> AddTransactionDependencyAsync(Transaction transactionThatNeedsLock,
                                                        Transaction transactionThatHasLock,
                                                        LockType lockType,
                                                        string lockedObject)
        {
            //await _semaphoreSlim.WaitAsync();
            //try
            //{
            //generate the structure
            var waitForGraph = new WaitForGraph
            {
                LockObject = lockedObject,
                LockTable = lockedObject,
                LockType = lockType,
                TransactionThatHasLockId = transactionThatHasLock.TransactionId,
                TransactionThatWantsLockId = transactionThatNeedsLock.TransactionId
            };

            await _dependencyRepository.AddAsync(waitForGraph);

            return waitForGraph;
            //}

            //finally
            //{
            //    _semaphoreSlim.Release();

            //}
        }

        public async Task RemoveDependencyAsync(WaitForGraph waitForGraph)
        {
            //await _semaphoreSlim.WaitAsync();

            //try
            //{
            await _dependencyRepository.DeleteAsync(waitForGraph);
            //}
            //finally
            //{
            //    _semaphoreSlim.Release();
            //}
        }

        public async Task<IList<Transaction>> GetAllTransactionsAsync()
        {
            //await _semaphoreSlim.WaitAsync();
            //try
            //{
            return await _transactionRepository.GetAllAsync(new List<string>
                {
                    nameof(Transaction.Locks),
                    nameof(Transaction.Operations),
                    nameof(Transaction.WaitForGraphsWantsLocks),
                    nameof(Transaction.WaitForGraphsHasLocks)
                });
            //}
            //finally
            //{
            //    _semaphoreSlim.Release();
            //}
        }

        public async Task<IList<Lock>> GetAllLocksAsync()
        {
            //await _semaphoreSlim.WaitAsync();
            //try
            //{
            return await _lockRepository.GetAllAsync(new List<string>
                {
                    nameof(Lock.Transaction)
                });
            //}

            //finally
            //{
            //    _semaphoreSlim.Release();
            //}
        }

        public async Task<IList<WaitForGraph>> GetAllWaitForGraphsAsync()
        {
            //await _semaphoreSlim.WaitAsync();

            //try
            //{
            return await _dependencyRepository.GetAllAsync(new List<string>
                {
                    nameof(WaitForGraph.TransactionThatHasLock),
                    nameof(WaitForGraph.TransactionThatWantsLock),
                });
            //}
            //finally
            //{
            //    _semaphoreSlim.Release();
            //}
        }
    }
}
