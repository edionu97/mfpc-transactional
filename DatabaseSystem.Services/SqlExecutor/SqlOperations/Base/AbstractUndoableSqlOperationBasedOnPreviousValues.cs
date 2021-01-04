using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatabaseSystem.Utility.Attributes;
using DatabaseSystem.Utility.Helpers;
using Microsoft.Data.SqlClient;

namespace DatabaseSystem.Services.SqlExecutor.SqlOperations.Base
{
    public abstract class AbstractUndoableSqlOperationBasedOnPreviousValues<T> : AbstractSqlOperation where T : new()
    {
        protected readonly List<SqlParameter> UndoParameters
            = new List<SqlParameter>();

        public IList<SqlParameter> ExtraParametersRequiredForUndo { get; set; }
            = new List<SqlParameter>();

        public string UndoDatabaseQuery { set; protected get; }

        public string SelectExistingDataQuery { set; protected get; }

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

            if (SelectExistingDataQuery == null)
            {
                throw new Exception("The select previous property must not be null");
            }

            //execute the select query first
            await using (var sqlDataReader = await executorService.ExecuteReadingQueryAsync(SelectExistingDataQuery, CommandParameters.ToArray()))
            {
                //it will be only one element
                await foreach (var @object in ConvertorHelpers.ConvertSqlRowsToObjects<T>(sqlDataReader))
                {
                    //add the parameters
                    UndoParameters.AddRange(
                        await ConvertorHelpers.ConvertObjectPropertiesIntoSqlParametersUsing<MapAttribute>(@object));
                }
            }

            //execute the delete
            await executorService
                .ExecuteModifyingOperationAsync(DatabaseQuery, CommandParameters.ToArray());

            //mark the operation
            IsExecutedSuccessfully = true;
        }

        public override async Task UndoAsync(ISqlExecutorService executorService)
        {
            if (IsExecutedSuccessfully)
            {
                return;
            }

            //execute the insert operation
            await executorService
                .ExecuteModifyingOperationAsync(
                    UndoDatabaseQuery,
                    ExtraParametersRequiredForUndo
                        .Concat(UndoParameters)
                        .ToArray());
        }
    }
}
