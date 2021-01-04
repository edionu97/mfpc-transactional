using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseSystem.Services.SqlExecutor.SqlOperations.Base
{
    public abstract class AbstractSqlQueryResultOperation<T> : AbstractSqlOperation
    {
        protected readonly IList<T> Result = new List<T>();

        [NotMapped]
        public IList<T> ComputedResult => IsExecutedSuccessfully ? Result : new List<T>();
    }
}
