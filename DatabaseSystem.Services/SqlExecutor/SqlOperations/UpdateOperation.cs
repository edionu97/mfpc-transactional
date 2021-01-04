using System;
using System.Linq;
using System.Threading.Tasks;
using DatabaseSystem.Services.SqlExecutor.SqlOperations.Base;
using DatabaseSystem.Utility.Attributes;
using DatabaseSystem.Utility.Helpers;

namespace DatabaseSystem.Services.SqlExecutor.SqlOperations
{
    public class UpdateOperation<T> : AbstractUndoableSqlOperationBasedOnPreviousValues<T> where T : new()
    {
    }
}
