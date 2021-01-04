using DatabaseSystem.Services.SqlExecutor.SqlOperations.Base;

namespace DatabaseSystem.Services.SqlExecutor.SqlOperations
{
    public class DeleteOperation<T> : AbstractUndoableSqlOperationBasedOnPreviousValues<T> where T : new()
    {
    }
}
