using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace DatabaseSystem.Services.SqlExecutor.Impl
{
    public class SqlExecutorService : ISqlExecutorService
    {
        private readonly string _connectionString;

        public SqlExecutorService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<int> ExecuteModifyingOperationAsync(string sqlQuery, params SqlParameter[] parameters)
        {
            //create the connection
            await using var sqlConnection = new SqlConnection(_connectionString);

            //create the command
            await using var sqlCommand = new SqlCommand(sqlQuery, sqlConnection)
            {
                CommandType = CommandType.Text
            };
            sqlCommand.Parameters.AddRange(parameters);

            //open the connection
            sqlConnection.Open();

            //get the result
            return await Task
                .Run(() => sqlCommand.ExecuteNonQuery())
                .ContinueWith(t =>
                {
                    sqlCommand.Parameters.Clear();
                    return t.Result;
                });
        }

        public async Task<object> ExecuteScalarAsync(string sqlQuery, params SqlParameter[] parameters)
        {
            //create the connection
            await using var sqlConnection = new SqlConnection(_connectionString);

            //create the command
            await using var sqlCommand = new SqlCommand(sqlQuery, sqlConnection)
            {
                CommandType = CommandType.Text
            };
            sqlCommand.Parameters.AddRange(parameters);

            //open the connection
            sqlConnection.Open();
            return await Task
                .Run(() => sqlCommand.ExecuteScalar())
                .ContinueWith(t =>
                {
                    sqlCommand.Parameters.Clear();
                    return t.Result;
                });
        }

        public async Task<SqlDataReader> ExecuteReadingQueryAsync(string sqlQuery, params SqlParameter[] parameters)
        {
            //create the connection
            var sqlConnection = new SqlConnection(_connectionString);

            //create the sql command
            await using var sqlCommand = new SqlCommand(sqlQuery, sqlConnection)
            {
                CommandType = CommandType.Text
            };
            sqlCommand.Parameters.AddRange(parameters);

            //open the connection
            sqlConnection.Open();
            
            //return the reader
            return await Task
                .Run(() => sqlCommand.ExecuteReader(CommandBehavior.CloseConnection))
                .ContinueWith(t =>
                {
                    sqlCommand.Parameters.Clear();
                    return t.Result;
                });
        }
    }
}
