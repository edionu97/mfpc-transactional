﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DatabaseSystem.Persistence.Models;
using DatabaseSystem.Services.Management;
using DatabaseSystem.Services.OperationBuilder;
using DatabaseSystem.Services.OperationBuilder.Impl;
using DatabaseSystem.Services.SqlExecutor;
using DatabaseSystem.Services.SqlExecutor.SqlOperations.Base;
using DatabaseSystem.Transactional.Graph;
using DatabaseSystem.Transactional.Transactional;

namespace DatabaseSystem.Services.Scheduling.Impl
{
    public partial class SchedulingService : ISchedulingService
    {
        private volatile IGraph _graph;
        private readonly ISqlExecutorService _sqlExecutor;
        private readonly IManagementService _managementService;
        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);

        public SchedulingService(ISqlExecutorService executorService, IGraph graph, IManagementService managementService)
        {
            _sqlExecutor = executorService;
            _graph = graph;
            _managementService = managementService;

            //create the long running task
            Task.Factory.StartNew(FindDeadlocks, TaskCreationOptions.LongRunning);
        }

        public ITransactionOperationsBuilder TransactionBuilder => new TransactionOperationsBuilder();

        public async Task<IList<AbstractSqlOperation>> ScheduleAndExecuteTransactionAsync(
            IList<Tuple<Operation, Lock, int?>> transactionOperations,
            Action<IList<AbstractSqlOperation>> continuation = null)
        {
            //create a transaction with operations
            var currentTransaction =
                await _managementService.CreateTransactionAsync(
                    transactionOperations.Select(x => x.Item1).ToList());

            var transactionResult = new List<AbstractSqlOperation>();
            try
            {
                //iterate the transaction operations
                for (var index = 0; index < transactionOperations.Count; ++index)
                {
                    //destruct the object
                    var (operation, @lock, time) = transactionOperations[index];

                    //only for debug purpose
                    if (time.HasValue)
                    {
                        await Task.Delay(time.Value);
                    }

                    //wait until you can acquire the lock
                    await WaitUntilCanAcquireLock(currentTransaction, @lock);

                    //critical section (one or more transaction could exit the waiting state but it is not sure that all of them should acquire the lock)
                    await _semaphoreSlim.WaitAsync();
                    try
                    {
                        //if there are transactions that are in opposition with this one that retry
                        if ((await GetAllOppositeTransactionsAsync(currentTransaction, @lock)).Any())
                        {
                            --index;
                            continue;
                        }

                        //acquire the lock
                        await _managementService
                            .AcquireLockAsync(currentTransaction, @lock.LockType, @lock.TableName);
                    }
                    finally
                    {
                        _semaphoreSlim.Release();
                    }

                    //execute operation
                    await ExecuteOperationAsync(operation, index, currentTransaction);

                    //if the operation is not an abstract sql operation then continue
                    if (!(operation is AbstractSqlOperation abstractSql))
                    {
                        continue;
                    }

                    transactionResult.Add(abstractSql);
                    continuation?.Invoke(transactionResult);
                }

            }
            catch (TaskCanceledException)
            {
                Console.WriteLine(
                    $"The transaction: {currentTransaction.TransactionId} has been chosen as deadlock victim");

                //execute the rollback
                await ExecuteRollbackAsync(transactionOperations);

                //throw back the exception
                throw new Exception(
                    $"The transaction: {currentTransaction.TransactionId} has been chosen as deadlock victim");
            }
            catch (Exception)
            {
                //execute the rollback
                await ExecuteRollbackAsync(transactionOperations);

                //propagate the error
                throw;
            }
            finally
            {
                //remove the transaction
                await RemoveTransactionAsync(currentTransaction);
            }

            return transactionResult;
        }

        private async Task RemoveTransactionAsync(Transaction transaction)
        {
            //remove the transaction
            await _managementService.RemoveTransactionAsync(transaction);

            //remove the vertex from dependency graph
            _graph.RemoveVertex(new SimpleGraphElement
            {
                Id = transaction.TransactionId
            });

            //remove the transaction from dependency graph
            while (_transactionWaitingTask.ContainsKey(transaction.TransactionId))
            {
                _transactionWaitingTask.TryRemove(transaction.TransactionId, out _);
            }
        }

        private async Task ExecuteOperationAsync(Operation operation, int index, Transaction currentTransaction)
        {
            Console.WriteLine($"Executing operation {index} from  transaction{currentTransaction.TransactionId}...");

            //execute only the sql operations
            if (operation is AbstractSqlOperation abstractSql)
            {
                await abstractSql.DoAsync(_sqlExecutor);
            }

            Console.WriteLine($"Done operation {index} from transaction {currentTransaction.TransactionId}");
        }

        private async Task ExecuteRollbackAsync(IEnumerable<Tuple<Operation, Lock, int?>> transactionOperations)
        {
            //do the rollback in reverse order
            foreach (var (operation, _, _) in transactionOperations.Reverse())
            {
                if (!(operation is AbstractSqlOperation abstractSql))
                {
                    continue;
                }

                await abstractSql.UndoAsync(_sqlExecutor);
            }
        }
    }
}
