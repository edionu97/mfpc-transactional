using System.Linq;
using System.Threading.Tasks;
using DatabaseSystem.Services.SqlExecutor.SqlOperations.Base;
using DatabaseSystem.Utility.Helpers;

namespace DatabaseSystem.Services.SqlExecutor.SqlOperations
{
    public class SelectOperation<T> : AbstractSqlQueryResultOperation<T> where T : new()
    {
        public override async Task DoAsync(ISqlExecutorService executorService)
        {
            //check if the executor is not null
            if (executorService == null)
            {
                return;
            }

            //get the sql data reader
            await using var sqlDataReader =
                await executorService
                    .ExecuteReadingQueryAsync(DatabaseQuery, CommandParameters.ToArray());

            //iterate each instance of object
            await foreach (var @object in ConvertorHelpers.ConvertSqlRowsToObjects<T>(sqlDataReader))
            {
               Result.Add(@object);
            }

            //mark the query as completed
            IsExecutedSuccessfully = true;
        }

        public override Task UndoAsync(ISqlExecutorService executorService)
        {
            //do nothing
            return Task.CompletedTask;
        }
    }
}
