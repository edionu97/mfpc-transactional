using System.Collections.Generic;
using System.Threading.Tasks;
using DatabaseSystem.Persistence.Enums;
using DatabaseSystem.Persistence.Models;
using DatabaseSystem.Persistence.Repository;

namespace DatabaseSystem.Services.Impl
{
    public partial class ManagementService : IManagementService
    {

        private readonly IRepository<Transaction> _transactionRepository;
        private readonly IRepository<Lock> _lockRepository;
        private readonly IRepository<WaitForGraph> _dependencyRepository;


        public ManagementService(IRepository<WaitForGraph> dependencyRepository,
                                 IRepository<Lock> lockRepository,
                                 IRepository<Transaction> transactionRepository)
        {
            _dependencyRepository = dependencyRepository;
            _lockRepository = lockRepository;
            _transactionRepository = transactionRepository;
        }

        public Transaction CreateTransaction(IList<string> operations)
        {
            lock (_transactionRepository)
            {
                return CreateTransactionAsync(operations).Result;
            }
        }

        public void AcquireLock(Transaction transaction, LockType lockType, string lockedObject)
        {
            lock (_lockRepository)
            {
                AcquireLockAsync(transaction, lockType, lockedObject).Wait();
            }
        }

        public void ReleaseLock(Lock @lock)
        {
            lock (_lockRepository)
            {
                ReleaseLockAsync(@lock).Wait();
            }
        }

        public Transaction FindTransactionById(int transactionId)
        {
            lock (_transactionRepository)
            {
                return FindTransactionByIdAsync(transactionId).Result;
            }
        }

        public void AddTransactionDependency(Transaction transactionThatNeedsLock,
                                             Transaction transactionThatHasLock,
                                             LockType lockType,
                                             string lockedObject)
        {
            lock (_dependencyRepository)
            {
                AddTransactionDependencyAsync(transactionThatNeedsLock,
                    transactionThatHasLock,
                    lockType,
                    lockedObject)
                .Wait();
            }
        }


    }
}
