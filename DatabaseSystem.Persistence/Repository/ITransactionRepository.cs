using System.Collections.Generic;
using System.Threading.Tasks;
using DatabaseSystem.Persistence.Models;

namespace DatabaseSystem.Persistence.Repository
{
    public interface ITransactionRepository : IRepository<Transaction>
    {
        Task<IList<Transaction>>
            FindTransactionsThatAreBlockingAsync(int transactionThatWantsToBeExecuted, Lock desiredLock);
    }
}
