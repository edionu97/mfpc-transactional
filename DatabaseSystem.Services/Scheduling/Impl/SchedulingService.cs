using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using DatabaseSystem.Persistence.Models;
using DatabaseSystem.Services.Management;
using DatabaseSystem.Utility.Enums;

namespace DatabaseSystem.Services.Scheduling.Impl
{
    public class SchedulingService : ISchedulingService
    {
        private readonly IManagementService _managementService;

        public SchedulingService(IManagementService managementService)
        {
            _managementService = managementService;
        }

        public async Task ScheduleAndExecuteTransactionAsync(IList<Tuple<Operation, Lock>> transactionOperations)
        {
            //create a transaction with operations
            var currentTransaction =
                await _managementService.CreateTransactionAsync(
                    transactionOperations.Select(x => x.Item1).ToList());
            try
            {
                //iterate the transaction operations
                foreach (var (operation, @lock) in transactionOperations)
                {
                    //TODO execute queries on db + add the deadlock + implement the rollback mechanism + commit
                    //wait until you can acquire the lock
                    await WaitUntilCanAcquireLock(currentTransaction, @lock);

                    //acquire the lock
                    await _managementService.AcquireLockAsync(currentTransaction, @lock.LockType, @lock.TableName);


                    //execute operation
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// This method waits until a transaction can acquire the lock
        /// </summary>
        /// <param name="currentTransaction">the transaction that wants to acquire the lock</param>
        /// <param name="desiredLock">the lock</param>
        private async Task WaitUntilCanAcquireLock(Transaction currentTransaction, Lock desiredLock)
        {
            //prepare a call that will get all the opposite transactions
            Task<IList<Transaction>> GetAllOppositeTransactions(Lock @lock)
                => _managementService.FindTransactionsThatAreBlockingAsync(currentTransaction.TransactionId, @lock);

            //get the transactions that are blocking the current transaction
            var blockingTransactions = await GetAllOppositeTransactions(desiredLock);
            
            //if there are no blocking transactions that do nothing
            if (!blockingTransactions.Any())
            {
                return;
            }

            //insert into dependency graph
            var dependencies = new List<WaitForGraph>();
            foreach (var blockingTransaction in blockingTransactions)
            {
                dependencies.Add(
                    await _managementService.AddTransactionDependencyAsync(
                        currentTransaction,
                        blockingTransaction,
                        desiredLock.LockType,
                        desiredLock.TableName));
            }

            //todo kill the task if deadlock
            //wait until there are no blocking transactions
            while ((await GetAllOppositeTransactions(desiredLock)).Any())
            {
                Console.WriteLine("Waiting...");
                await Task.Delay(25);
            }

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
    }
}
