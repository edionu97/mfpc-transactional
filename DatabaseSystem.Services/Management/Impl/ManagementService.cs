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


        public ManagementService(IRepository<Lock> lockRepository,
                                 ITransactionRepository transactionRepository)
        {
            _lockRepository = lockRepository;
            _transactionRepository = transactionRepository;
        }

        public async Task<Transaction> CreateTransactionAsync(IList<Operation> operations)
        {
            var transaction = new Transaction();
            foreach (var operation in operations)
            {
                transaction.Operations.Add(operation);
            }

            //add the transaction and return the result
            await _transactionRepository.AddAsync(transaction);
            return transaction;

        }

        public async Task RemoveTransactionAsync(Transaction transaction)
        {
            await _transactionRepository.DeleteAsync(transaction);
        }

        public async Task AcquireLockAsync(Transaction transaction, LockType lockType, string lockedObject)
        {
            var @lock = new Lock
            {
                Object = lockedObject,
                TableName = lockedObject,
                LockType = lockType,
                TransactionId = transaction.TransactionId
            };

            //add the lock
            await _lockRepository.AddAsync(@lock);
        }

        public async Task ReleaseLockAsync(Lock @lock)
        {
            await _lockRepository.DeleteAsync(@lock);
        }

        public async Task<Transaction> FindTransactionByIdAsync(int transactionId)
        {
            return await _transactionRepository.FindByIdAsync(transactionId, new List<string>
                {
                    nameof(Transaction.Locks),
                    nameof(Transaction.Operations),
                });
        }

        public async Task<IList<Transaction>> FindTransactionsThatAreBlockingAsync(int transactionThatWantsToBeExecuted, Lock desiredLock)
        {
            return
                await _transactionRepository
                    .FindTransactionsThatAreBlockingAsync(transactionThatWantsToBeExecuted, desiredLock);
        }

        public async Task<IList<Transaction>> GetAllTransactionsAsync()
        {
            return await _transactionRepository.GetAllAsync(new List<string>
                {
                    nameof(Transaction.Locks),
                    nameof(Transaction.Operations),
                });
        }

        public async Task<IList<Lock>> GetAllLocksAsync()
        {
            return await _lockRepository.GetAllAsync(new List<string>
                {
                    nameof(Lock.Transaction)
                });
        }
    }
}
