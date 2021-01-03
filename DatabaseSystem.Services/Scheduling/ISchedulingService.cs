using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DatabaseSystem.Persistence.Models;

namespace DatabaseSystem.Services.Scheduling
{
    public interface ISchedulingService
    {
        /// <summary>
        /// This method it is used for schedule and execute transactions
        /// </summary>
        /// <param name="transactionOperations">
        ///     the list of operation of a transaction
        ///     this is a list of tuple (first item is the operation, second is the lock type and third it is used in for debugging purpose)
        /// </param>
        Task ScheduleAndExecuteTransactionAsync(IList<Tuple<Operation, Lock, int?>> transactionOperations);
    }
}
