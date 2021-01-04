using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Threading.Tasks;
using DatabaseSystem.Persistence.Models;
using Microsoft.Data.SqlClient;

namespace DatabaseSystem.Services.SqlExecutor.SqlOperations.Base
{
    public abstract class AbstractSqlOperation : Operation
    {
        public IList<SqlParameter> CommandParameters { get; set; } = new List<SqlParameter>();

        [NotMapped]
        public bool IsExecutedSuccessfully { protected set; get; }

        public abstract Task DoAsync(ISqlExecutorService executorService);

        public abstract Task UndoAsync(ISqlExecutorService executorService);

        public void AddParameter(string parameterName, string parameterValue, SqlDbType parameterType)
        {
            CommandParameters.Add(new SqlParameter(parameterName, parameterType)
            {
                Value = parameterValue
            });
        }
    }
}
