using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DatabaseSystem.Persistence.Models;
using DatabaseSystem.Transactional.Transactional;

namespace DatabaseSystem.Services.Scheduling.Impl
{
    public partial class SchedulingService
    {
        /// <summary>
        /// This method waits until a transaction can acquire the lock
        /// </summary>
        /// <param name="currentTransaction">the transaction that wants to acquire the lock</param>
        /// <param name="desiredLock">the lock</param>
        private async Task WaitUntilCanAcquireLock(Transaction currentTransaction, Lock desiredLock)
        {
            //get the transactions that are blocking the current transaction
            var blockingTransactions = await GetAllOppositeTransactionsAsync(currentTransaction, desiredLock);

            //if there are no blocking transactions that do nothing
            if (!blockingTransactions.Any())
            {
                return;
            }

            //insert into dependency graph
            var dependencies = new List<WaitForGraph>();
            foreach (var blockingTransaction in blockingTransactions)
            {
                //add dependency directed edge 
                _graph.InsertEdge(
                    new SimpleGraphElement
                    {
                        Id = currentTransaction.TransactionId
                    },
                    new SimpleGraphElement
                    {
                        Id = blockingTransaction.TransactionId
                    });

                //add into database the dependency
                dependencies.Add(
                    await _managementService.AddTransactionDependencyAsync(
                        currentTransaction,
                        blockingTransaction,
                        desiredLock.LockType,
                        desiredLock.TableName));
            }

            //wait until the task is completed
            await PutTheadOnWaitingAsync(currentTransaction, desiredLock);

            //remove all dependencies
            foreach (var dependency in dependencies)
            {
                try
                {
                    await _managementService.RemoveDependencyAsync(dependency);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        /// <summary>
        /// Finds for a transaction all the other transactions that contain locks that are incompatible with the desired lock 
        /// </summary>
        /// <param name="currentTransaction">the current transaction</param>
        /// <param name="lock">the desired lock</param>
        /// <returns>as list of transactions</returns>
        private Task<IList<Transaction>> GetAllOppositeTransactionsAsync(Transaction currentTransaction, Lock @lock)
            => _managementService.FindTransactionsThatAreBlockingAsync(currentTransaction.TransactionId, @lock);


        private Task PutTheadOnWaitingAsync(Transaction currentTransaction, Lock desiredLock)
        {
            //create the graph element
            var transactionalElement = new TransactionalGraphElement
            {
                Id = currentTransaction.TransactionId,
                CancellationTokenSource = new CancellationTokenSource(),
            };
            transactionalElement.CancellationToken = transactionalElement.CancellationTokenSource.Token;

            //create the waiting task
            transactionalElement.ActiveTask = Task.Run(
                async () =>
                {
                    //get the token
                    var token = transactionalElement.CancellationToken;

                    //wait until there are no blocking transactions
                    while (!token.IsCancellationRequested
                           && (await GetAllOppositeTransactionsAsync(currentTransaction, desiredLock)).Any())
                    {
                        Console.WriteLine("Waiting... " + currentTransaction.TransactionId);
                        await Task.Delay(25, token);
                    }

                }, transactionalElement.CancellationToken);

            //add into the dictionary
            // ReSharper disable once InvertIf
            if (!_transactionWaitingTask.ContainsKey(transactionalElement.Id))
            {
                while (!_transactionWaitingTask.TryAdd(transactionalElement.Id, transactionalElement))
                {
                    //loop
                }
            }

            return transactionalElement.ActiveTask;
        }
    }
}
