using System;
using System.Collections.Generic;
using System.Linq;
using DatabaseSystem.Persistence.Models;
using DatabaseSystem.Services.SqlExecutor.SqlOperations;
using DatabaseSystem.Services.SqlExecutor.SqlOperations.Base;
using DatabaseSystem.Utility.Enums;
using Microsoft.Data.SqlClient;

namespace DatabaseSystem.Services.OperationBuilder.Impl
{
    public class TransactionOperationsBuilder : ITransactionOperationsBuilder
    {
        private readonly IList<Tuple<Operation, Lock, int?>> _operations
            = new List<Tuple<Operation, Lock, int?>>();

        public ITransactionOperationsBuilder AddSelectQuery<T>(
            string selectQueryString,
            string table, IList<SqlParameter> cmdParameters) where T : new()
        {
            //add the operation into the list
            _operations.Add(
                Tuple.Create<Operation, Lock, int?>(
                new SelectOperation<T>
                {
                    CommandParameters = cmdParameters,
                    DatabaseQuery = selectQueryString
                },
                new Lock
                {
                    LockType = LockType.Read,
                    TableName = table,
                    Object = table
                },
                null));

            //return the instance
            return this;
        }

        public ITransactionOperationsBuilder AddInsertQuery(
            string insertQueryString,
            string undoQueryString,
            string table,
            IList<SqlParameter> insertQueryParameters, SqlParameter undoQueryParameter)
        {

            //add the operation into the list
            _operations.Add(
                Tuple.Create<Operation, Lock, int?>(
                new InsertOperation
                {
                    //the query that will be executed on db
                    DatabaseQuery = insertQueryString,

                    //the command parameters
                    CommandParameters = insertQueryParameters,

                    //the undo query
                    UndoDatabaseQuery = undoQueryString,
                    UndoParameter = undoQueryParameter
                },
                new Lock
                {
                    LockType = LockType.Write,
                    TableName = table,
                    Object = table
                },
                null));

            //return the instance
            return this;
        }

        public ITransactionOperationsBuilder AddDeleteQuery<T>(
            string deleteQueryString,
            string undoQueryString,
            string selectPreviousDataQueryString,
            string tableName, 
            IList<SqlParameter> commandParameters) where T: new()
        {
            //add the delete query
            _operations.Add(
                Tuple.Create<Operation, Lock, int?>(
                    new DeleteOperation<T>
                    {
                        DatabaseQuery = deleteQueryString,

                        SelectExistingDataQuery = selectPreviousDataQueryString,
                        UndoDatabaseQuery = undoQueryString,

                        CommandParameters = commandParameters
                    },
                    new Lock
                    {
                        LockType = LockType.Write,
                        TableName = tableName,
                        Object = tableName
                    },
                    null));

            return this;
        }

        public ITransactionOperationsBuilder GetOperationParameters(out IDictionary<int, IDictionary<string, SqlParameter>> parameters)
        {
            //instantiate the parameters
            parameters = new Dictionary<int, IDictionary<string, SqlParameter>>();

            //add the parameters
            for (var opIdx = 0; opIdx < _operations.Count; ++opIdx)
            {
                //decompose the operation
                var (operation, _, _) = _operations[opIdx];

                //check if the operation is an operation that has parameters
                if (!(operation is AbstractSqlOperation abstractSql))
                {
                    continue;
                }

                //convert the parameters from list into dictionary
                var parametersAsDictionary = 
                    abstractSql
                        .CommandParameters
                        .ToDictionary(
                            x => x.ParameterName, 
                            x => x);

                //add into the parameters the command parameters
                parameters.Add(opIdx + 1, parametersAsDictionary);
            }

            return this;
        }

        public IList<Tuple<Operation, Lock, int?>> BuildTransaction()
        {
            return _operations;
        }
    }
}
