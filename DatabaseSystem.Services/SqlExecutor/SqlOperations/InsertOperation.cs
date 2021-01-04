using System;
using System.Linq;
using System.Threading.Tasks;
using DatabaseSystem.Services.SqlExecutor.SqlOperations.Base;
using Microsoft.Data.SqlClient;

namespace DatabaseSystem.Services.SqlExecutor.SqlOperations
{
    public class InsertOperation : AbstractSqlOperation
    {
        public string UndoDatabaseQuery { set; protected get; }

        public SqlParameter UndoParameter { protected get; set; }

        public override async Task DoAsync(ISqlExecutorService executorService)
        {
            //check if the executor is not null
            if (executorService == null)
            {
                return;
            }

            if (UndoDatabaseQuery == null)
            {
               throw new Exception("The undo property must not be null");
            }

            if (UndoParameter == null)
            {
                throw new Exception("The undo parameter must not be null");
            }

            //augment the query so that it will return the id of the inserted value
            //assumption: the id is generated using identity property
            var query = $"{DatabaseQuery};select scope_identity();";

            //insert the value into
            UndoParameter.Value = await executorService
                .ExecuteScalarAsync(query, CommandParameters.ToArray());

                //mark the operation as being completed
            IsExecutedSuccessfully = true;
        }

        public override async Task UndoAsync(ISqlExecutorService executorService)
        {
            if (IsExecutedSuccessfully)
            {
                return;
            }

            //delete the inserted object
            await executorService
                .ExecuteModifyingOperationAsync(UndoDatabaseQuery, UndoParameter);
        }
    }
}
