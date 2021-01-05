using System;
using System.Collections.Generic;
using DatabaseSystem.Persistence.Models;
using Microsoft.Data.SqlClient;

namespace DatabaseSystem.Services.OperationBuilder
{
    public interface ITransactionOperationsBuilder
    {
        /// <summary>
        /// Add select query
        /// </summary>
        /// <typeparam name="T">the type of the operation</typeparam>
        /// <param name="selectQueryString">the select query string</param>
        /// <param name="table">the table on which the query will be executed</param>
        /// <param name="cmdParameters">the parameters of the sql command</param>
        /// <returns>the instance of the transaction builder</returns>
        public ITransactionOperationsBuilder AddSelectQuery<T>(
            string selectQueryString, 
            string table, IList<SqlParameter> cmdParameters) where T : new();

        /// <summary>
        /// This will add an insert query
        /// </summary>
        /// <param name="insertQueryString">the string that represent the insert query</param>
        /// <param name="table">the table on which we want to do the insert (parameters used for insert query)</param>
        /// <param name="insertQueryParameters">the parameters of the command</param>
        /// <param name="undoQueryString">the undo query</param>
        /// <param name="undoQueryParameter">the undo parameter from undo query</param>
        /// <returns>the instance of the transaction builder</returns>
        public ITransactionOperationsBuilder AddInsertQuery(
            string insertQueryString,
            string undoQueryString,
            string table,
            IList<SqlParameter> insertQueryParameters,
            SqlParameter undoQueryParameter);

        /// <summary>
        /// This will add a delete query
        /// </summary>
        /// <param name="deleteQueryString">the query itself</param>
        /// <param name="undoQueryString">the composite operation</param>
        /// <param name="selectPreviousDataQueryString">the data before delete</param>
        /// <param name="tableName">the name of the table</param>
        /// <param name="commandParameters">the parameters</param>
        /// <returns>the instance of the transaction builder</returns>
        public ITransactionOperationsBuilder AddDeleteQuery<T>(
            string deleteQueryString,
            string undoQueryString,
            string selectPreviousDataQueryString,
            string tableName,
            IList<SqlParameter> commandParameters) where T : new();


        /// <summary>
        /// This method it is used for getting all the command parameters
        /// </summary>
        /// <param name="parameters">this represents a dictionary (the key is the operation index and the value is a list of its parameters)</param>
        /// <returns>the instance</returns>
        public ITransactionOperationsBuilder GetOperationParameters(
            out IDictionary<int, IDictionary<string, SqlParameter>> parameters);

        /// <summary>
        /// Create a transaction from all the operation
        /// </summary>
        /// <returns>a list of operations</returns>
        public IList<Tuple<Operation, Lock, int?>> BuildTransaction();
    }
}
