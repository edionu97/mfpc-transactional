using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DatabaseSystem.Persistence.Models;

namespace DatabaseSystem.Services.Scheduling
{
    public interface ISchedulingService
    {
        Task ScheduleAndExecuteTransactionAsync(IList<Tuple<Operation, Lock, int>> transactionOperations);
    }
}
