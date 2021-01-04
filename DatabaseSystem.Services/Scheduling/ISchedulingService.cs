using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DatabaseSystem.Persistence.Models;
using DatabaseSystem.Services.SqlExecutor.SqlOperations.Base;

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
        Task<IList<AbstractSqlOperation>> ScheduleAndExecuteTransactionAsync(IList<Tuple<Operation, Lock, int?>> transactionOperations);
    }
}
