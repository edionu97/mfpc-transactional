﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseSystem.Persistence.Enums;
using DatabaseSystem.Persistence.Models;

namespace DatabaseSystem.Services.Impl
{
    public partial class ManagementService
    {
        public async Task<Transaction> CreateTransactionAsync(IList<string> operations)
        {
            //create the transaction
            var transaction = new Transaction();
            foreach (var operation in operations)
            {
                transaction.Operations.Add(new Operation
                {
                    DatabaseQuery = operation
                });
            }

            //add the transaction and return the result
            await _transactionRepository.AddAsync(transaction);
            return transaction;
        }

        public void RemoveTransaction(Transaction transaction)
        {
            lock (_transactionRepository)
            {
                lock (_dependencyRepository)
                {
                    RemoveTransactionAsync(transaction).Wait();
                }
            }
        }

        public async Task RemoveTransactionAsync(Transaction transaction)
        {
            foreach (var waitForGraph in transaction.WaitForGraphsWantsLocks.Concat(transaction.WaitForGraphsHasLocks).ToList())
            {
                await _dependencyRepository.DeleteAsync(waitForGraph);
            }

            await _transactionRepository.DeleteAsync(transaction);
        }

        public async Task AcquireLockAsync(Transaction transaction, LockType lockType, string lockedObject)
        {
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
                nameof(Transaction.WaitForGraphsWantsLocks),
                nameof(Transaction.WaitForGraphsHasLocks)
            });
        }

        public async Task AddTransactionDependencyAsync(Transaction transactionThatNeedsLock,
                                                        Transaction transactionThatHasLock,
                                                        LockType lockType, 
                                                        string lockedObject)
        {
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
        }

        public async Task<IList<Transaction>> GetAllTransactionsAsync()
        {
            return await _transactionRepository.GetAllAsync(new List<string>
            {
                nameof(Transaction.Locks),
                nameof(Transaction.Operations),
                nameof(Transaction.WaitForGraphsWantsLocks),
                nameof(Transaction.WaitForGraphsHasLocks)
            });
        }

        public async Task<IList<Lock>> GetAllLocksAsync()
        {
            return await _lockRepository.GetAllAsync(new List<string>
            {
                nameof(Lock.Transaction)
            });
        }

        public async Task<IList<WaitForGraph>> GetAllWaitForGraphsAsync()
        {
            return await _dependencyRepository.GetAllAsync(new List<string>
            {
                nameof(WaitForGraph.TransactionThatHasLock),
                nameof(WaitForGraph.TransactionThatWantsLock),
            });
        }
    }
}