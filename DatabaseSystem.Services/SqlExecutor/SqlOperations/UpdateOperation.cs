using DatabaseSystem.Services.SqlExecutor.SqlOperations.Base;

namespace DatabaseSystem.Services.SqlExecutor.SqlOperations
{
    public class UpdateOperation<T> : AbstractUndoableSqlOperationBasedOnPreviousValues<T> where T : new()
    {
    }
}
