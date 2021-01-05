using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseSystem.Persistence.Models;
using DatabaseSystem.Services.OperationBuilder;
using DatabaseSystem.Services.SqlExecutor.SqlOperations;
using DatabaseSystem.Services.SqlExecutor.SqlOperations.Base;
using DatabaseSystem.Utility.Enums;
using Microsoft.Data.SqlClient;
using OnlineShopping.Models;

namespace OnlineShopping.Services.Impl
{
    public partial class ShoppingService
    {

        /// <summary>
        /// Helper method for creating the transaction for method add product
        /// </summary>
        /// <returns>the created transaction builder</returns>
        public ITransactionOperationsBuilder GetTransactionBuilderForAddProduct(
            string productName,
            int price,
            string model,
            int productCode)
        {
            //create a new instance of transaction builder
            var transactionBuilder = _schedulingService.TransactionBuilder;

            //add data into builder
            return transactionBuilder
                //add the select query
                .AddSelectQuery<Product>(
                    "select * from Product where ProductCode = @productCode",
                    nameof(Product),
                    new List<SqlParameter>
                    {
                        new SqlParameter("@productCode", SqlDbType.Int)
                        {
                            Value = productCode
                        }

                    })
                //add the insert query
                .AddInsertQuery(
                    @"insert into Product values (
                            @productName,
                            @price, 
                            @model,
                            @productCode)",
                    "delete from Product where ProductId = @productId",
                    nameof(Product),
                    new List<SqlParameter>
                    {
                        new SqlParameter("@productName", SqlDbType.NVarChar)
                        {
                            Value = productName
                        },
                        new SqlParameter("@price", SqlDbType.Int)
                        {
                            Value = price
                        },
                        new SqlParameter("@model", SqlDbType.NVarChar)
                        {
                            Value = model
                        },
                        new SqlParameter("@productCode", SqlDbType.Int)
                        {
                            Value = productCode
                        }
                    },
                    new SqlParameter("@productId", SqlDbType.Int)
                );
        }

        /// <summary>
        /// Helper method for creating the transaction for method register client
        /// </summary>
        /// <returns>the created transaction builder</returns>
        public ITransactionOperationsBuilder GetTransactionBuilderForRegisterClient(
            string clientFirstName,
            string clientLastName,
            string clientCnp,
            string phoneNumber,
            string email)
        {
            //create the transaction builder
            var transactionBuilder = _schedulingService.TransactionBuilder;

            //build the transaction
            transactionBuilder
                .AddSelectQuery<Client>(
                    "select * from Client where CNP = @clientCnp",
                    nameof(Client),
                    new List<SqlParameter>
                    {
                        new SqlParameter("@clientCnp", SqlDbType.NVarChar)
                        {
                            Value = clientCnp
                        }
                    })
                .AddInsertQuery(
                    @"insert into Client values (
                            @clientFirstName,
                            @clientLastName, 
                            @clientCnp,
                            @phoneNumber,
                            @email
                        )",
                    "delete from Client where ClientId = @clientId",
                    nameof(Client),
                    new List<SqlParameter>
                    {
                        new SqlParameter("@clientFirstName", SqlDbType.NVarChar)
                        {
                            Value = clientFirstName
                        },
                        new SqlParameter("@clientLastName", SqlDbType.NVarChar)
                        {
                            Value = clientLastName
                        },
                        new SqlParameter("@clientCnp", SqlDbType.NVarChar)
                        {
                            Value = clientCnp
                        },
                        new SqlParameter("@phoneNumber", SqlDbType.NVarChar)
                        {
                            Value = phoneNumber
                        },
                        new SqlParameter("@email", SqlDbType.NVarChar)
                        {
                            Value = email
                        }
                    },
                    new SqlParameter("@clientId", SqlDbType.Int)
                );

            //return the builder
            return transactionBuilder;
        }

        /// <summary>
        /// Helper method for creating the transaction for method add order
        /// </summary>
        /// <returns>the created transaction builder</returns>
        public ITransactionOperationsBuilder GetTransactionBuilderForAddOrder(
            int clientId,
            int productId,
            int numberOfItems)
        {
            //create a new transaction builder
            var transactionBuilder = _schedulingService.TransactionBuilder;

            //create the transaction
            transactionBuilder
                .AddInsertQuery(
                    @"insert into Orders values (
                            @clientId,
                            @productId, 
                            @dateValue, 
                            @numberOfItems)",
                    "delete from Orders where OrderId = @orderId",
                    $"{nameof(Order)}s",
                    new List<SqlParameter>
                    {
                        new SqlParameter("@clientId", SqlDbType.Int)
                        {
                            Value = clientId
                        },
                        new SqlParameter("@productId", SqlDbType.Int)
                        {
                            Value = productId
                        },
                        new SqlParameter("@dateValue", SqlDbType.DateTime)
                        {
                            Value = DateTime.Now
                        },
                        new SqlParameter("@numberOfItems", SqlDbType.Int)
                        {
                            Value = numberOfItems
                        }
                    },
                    new SqlParameter("@orderId", SqlDbType.Int)
                );

            //return the builder
            return transactionBuilder;
        }

        /// <summary>
        /// Get the transaction builder required 
        /// </summary>
        /// <param name="clientCnp"></param>
        /// <returns></returns>
        public ITransactionOperationsBuilder GetTransactionBuilderForAllClientOrders(string clientCnp)
        {
            //create the transaction builder
            var transactionBuilder = _schedulingService.TransactionBuilder;

            //create the transaction
            transactionBuilder
                .AddSelectQuery<Client>(
                    "select top(1) * from Client where CNP = @cnp",
                    nameof(Client),
                    new List<SqlParameter>
                    {
                        new SqlParameter("@cnp", SqlDbType.NVarChar)
                        {
                            Value = clientCnp
                        }
                    })
                .AddSelectQuery<Order>(
                    "select * from Orders where ClientId = @clientId",
                    $"{nameof(Order)}s",
                    new List<SqlParameter>
                    {
                        new SqlParameter("@clientId", SqlDbType.Int)
                    })
                .AddSelectQuery<Product>(
                    "select * from Product where ProductId in ($placeholder$)",
                    nameof(Product), 
                    new List<SqlParameter>());

            //return the builder
            return transactionBuilder;
        }

        private ITransactionOperationsBuilder GetTransactionBuilderForAllProducts()
        {
            //create the transaction builder
            var transactionBuilder = _schedulingService.TransactionBuilder;

            //create the transaction
            transactionBuilder
                .AddSelectQuery<Product>(
                    "select * from Product",
                    nameof(Product),
                    new List<SqlParameter>());

            //return the builder
            return transactionBuilder;
        }

        private ITransactionOperationsBuilder GetTransactionBuilderForRemoveProductFromOrder(int clientId, int productId)
        {
            //get the transaction builder
            var transactionBuilder = 
                _schedulingService.TransactionBuilder;

            //create the transaction
            transactionBuilder
                .AddSelectQuery<Order>(
                    "select top(1) * from Orders where ClientId = @clientId and ProductId = @productId",
                    $"{nameof(Order)}s",
                    new List<SqlParameter>
                    {
                        new SqlParameter("@clientId", SqlDbType.Int)
                        {
                            Value = clientId
                        },
                        new SqlParameter("@productId", SqlDbType.Int)
                        {
                            Value = productId
                        }
                    })
                .AddDeleteQuery<Order>(
                    "delete from Orders where OrderId = @orderId",
                    "insert into Orders values(@ClientId, @ProductId, @OrderDate, @ItemsNo)",
                    "select top(1) * from Orders where OrderId = @orderId",
                    $"{nameof(Order)}s",
                    new List<SqlParameter>
                    {
                        new SqlParameter("@orderId", SqlDbType.Int)
                    });


            //return the builder
            return transactionBuilder;
        }
    }
}
