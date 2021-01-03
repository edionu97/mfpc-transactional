using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DatabaseSystem.Persistence.Models;
using DatabaseSystem.Services.Management;
using DatabaseSystem.Transactional.Graph;

namespace DatabaseSystem.Services.Scheduling.Impl
{
    public partial class SchedulingService : ISchedulingService
    {
        private volatile IGraph _graph;
        private readonly IManagementService _managementService;
        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);

        public SchedulingService(IGraph graph, IManagementService managementService)
        {
            _graph = graph;
            _managementService = managementService;

            //create the long running task
            Task.Factory.StartNew(FindDeadlocks, TaskCreationOptions.LongRunning);
        }

        public async Task ScheduleAndExecuteTransactionAsync(IList<Tuple<Operation, Lock, int>> transactionOperations)
        {
            //create a transaction with operations
            var currentTransaction =
                await _managementService.CreateTransactionAsync(
                    transactionOperations.Select(x => x.Item1).ToList());
            try
            {
                //iterate the transaction operations
                for (var index = 0; index < transactionOperations.Count; ++index)
                {
                    //destruct the object
                    var (operation, @lock, time) = transactionOperations[index];

                    await Task.Delay(time);

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
                }

            }
            catch (TaskCanceledException)
            {
                Console.WriteLine(
                    $"The transaction: {currentTransaction.TransactionId} has been chosen as deadlock victim");

                //execute abort code (rollback)
            }
            finally
            {
                //remove the transaction
                await _managementService.RemoveTransactionAsync(currentTransaction);
            }
        }
    }
}
