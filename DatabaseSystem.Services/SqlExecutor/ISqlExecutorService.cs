using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace DatabaseSystem.Services.SqlExecutor
{
    public interface ISqlExecutorService
    {
        /// <summary>
        /// This method it is used for executing one of the UPDATE, INSERT or DELETE commands
        /// </summary>
        /// <param name="sqlQuery">the sql query</param>
        /// <param name="parameters">the parameters of the command</param>
        /// <returns></returns>
        public Task<int> ExecuteModifyingOperationAsync(string sqlQuery, params SqlParameter[] parameters);

        /// <summary>
        /// This method execute the command and returns the first column of the first row of the sqlQuery
        /// </summary>
        /// <param name="sqlQuery">the sql query</param>
        /// <param name="parameters">the command parameters</param>
        /// <returns>the first column</returns>
        public Task<object> ExecuteScalarAsync(string sqlQuery, params SqlParameter[] parameters);


        /// <summary>
        /// Execute the command with query and return the reader.
        /// The connection will be closed when the reader will be disposed 
        /// </summary>
        /// <param name="sqlQuery">the sql query</param>
        /// <param name="parameters">the parameters</param>
        /// <returns>the sql reader</returns>
        public Task<SqlDataReader> ExecuteReadingQueryAsync(string sqlQuery, params SqlParameter[] parameters);
    }
}
