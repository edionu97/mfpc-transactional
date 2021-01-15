using System;
using System.Linq;
using OnlineShopping.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using DatabaseSystem.Services.Scheduling;
using DatabaseSystem.Services.SqlExecutor.SqlOperations.Base;

namespace OnlineShopping.Services.Impl
{
    public partial class ShoppingService : IShoppingService
    {
        private readonly ISchedulingService _schedulingService;

        public ShoppingService(ISchedulingService schedulingService)
        {
            _schedulingService = schedulingService;
        }

        public async Task AddProductAsync(string productName,
                                          int price,
                                          string model,
                                          int productCode)
        {
            //get the instance of transaction builder of the current operation
            var transactionBuilder
                = GetTransactionBuilderForAddProduct(productName, price, model, productCode);

            //execute the transaction
            await _schedulingService
                .ScheduleAndExecuteTransactionAsync(
                    transactionBuilder.BuildTransaction(),
                    (executedOperationsFromTransaction) =>
                    {
                        //only the result of the previous operation is important for this query
                        if (executedOperationsFromTransaction.Count != 1)
                        {
                            return;
                        }

                        //get the select operation
                        var selectOperation =
                            (executedOperationsFromTransaction.First() as AbstractSqlQueryResultOperation<Product>);


                        //throw exception if in database already exist such an product
                        if (selectOperation?.ComputedResult.Any() == true)
                        {
                            throw new Exception("The in database already exists a product with the same code");
                        }
                    });
        }

        public async Task RegisterNewClientAsync(string clientFirstName,
                                                 string clientLastName,
                                                 string clientCnp,
                                                 string phoneNumber,
                                                 string email)
        {
            //get instance of transaction builder
            var transactionBuilder =
                GetTransactionBuilderForRegisterClient(
                    clientFirstName,
                    clientLastName,
                    clientCnp,
                    phoneNumber,
                    email);

            //execute the transaction
            await _schedulingService
                .ScheduleAndExecuteTransactionAsync(
                    transactionBuilder.BuildTransaction(),
                    (executedOperationsFromTransaction) =>
                    {
                        //only the result of the previous operation is important for this query
                        if (executedOperationsFromTransaction.Count != 1)
                        {
                            return;
                        }

                        //get the select operation
                        var selectOperation =
                            (executedOperationsFromTransaction.First() as AbstractSqlQueryResultOperation<Client>);

                        //throw exception if in database already exist such an product
                        if (selectOperation?.ComputedResult.Any() == true)
                        {
                            throw new Exception("The user already exists into database");
                        }
                    });
        }

        public async Task AddOrderAsync(int clientId, int productId, int numberOfItems)
        {
            //get the transaction builder
            var transactionBuilder = GetTransactionBuilderForAddOrder(
                clientId,
                productId,
                numberOfItems);

            //execute the transaction
            await _schedulingService
                .ScheduleAndExecuteTransactionAsync(transactionBuilder.BuildTransaction());
        }

        public async Task RemoveProductFromOrderAsync(int clientId, int productId)
        {
            //get the transaction builder
            var transactionBuilder
                = GetTransactionBuilderForRemoveProductFromOrder(clientId, productId)
                    .GetOperationParameters(out var parameters);

            //execute the operation
            await _schedulingService.ScheduleAndExecuteTransactionAsync(
                transactionBuilder.BuildTransaction(),
                (executedOperationsFromTransaction) =>
                {
                    //first operation will set data for the second operation
                    if (executedOperationsFromTransaction.Count != 1)
                    {
                        return;
                    }

                    //get the order
                    var order =
                        (executedOperationsFromTransaction
                            .First() as AbstractSqlQueryResultOperation<Order>)
                        ?.ComputedResult
                        .FirstOrDefault();

                    //check if order exists
                    if (order == null)
                    {
                        throw new Exception("In database does not exist such an order");
                    }

                    parameters[2]["@orderId"].Value = order.OrderId;
                });
        }

        public async Task<IList<Order>> GetOrdersForClientAsync(string clientCnp)
        {
            //get the transaction builder
            var transactionBuilder =
                GetTransactionBuilderForAllClientOrders(clientCnp)
                    .GetOperationParameters(out var parameters);

            //get the transactions operations
            var transactionOperation = 
                transactionBuilder
                    .BuildTransaction()
                    .Select(x => x.Item1)
                    .ToList();

            //execute the transaction and get the desired results
            var result = await _schedulingService
                .ScheduleAndExecuteTransactionAsync(
                    transactionBuilder.BuildTransaction(),
                    (executedOperationsFromTransaction) =>
                    {
                        //different code  based on the operation index
                        switch (executedOperationsFromTransaction.Count)
                        {
                            //configure the second operation from transaction
                            case 1:
                                {
                                    //get the client
                                    var desiredClient =
                                        (executedOperationsFromTransaction.First() as
                                            AbstractSqlQueryResultOperation<Client>)
                                        ?.ComputedResult
                                        ?.FirstOrDefault();

                                    //check if the client exists
                                    if (desiredClient == null)
                                    {
                                        throw new Exception("In database does not exist such a client");
                                    }

                                    //set the parameter for the next operation
                                    parameters[2]["@clientId"].Value = desiredClient.ClientId;
                                    return;
                                }

                            //configure the parameters for the third operation from transaction
                            case 2:
                                {
                                    //get the client orders
                                    var clientOrders =
                                        (executedOperationsFromTransaction
                                                .Skip(1)
                                                .First() as AbstractSqlQueryResultOperation<Order>)
                                        ?.ComputedResult;

                                    //treat the case in which there are no orders
                                    if (clientOrders?.Any() == false)
                                    {
                                        throw new Exception("The user does not have any orders");
                                    }

                                    //get the products ids
                                    var distinctProductIds =
                                        clientOrders?
                                            .Select(x => x.ProductId)
                                            .Select(x => x.ToString())
                                            .Distinct()
                                            .ToList();

                                    //get the next operation
                                    var nextOperation = transactionOperation
                                        .Skip(2)
                                        .First();

                                    //get the replaced query
                                    var replacedQuery = 
                                        nextOperation
                                            .DatabaseQuery
                                            .Replace(
                                            "$placeholder$",
                                            string
                                                .Join(',', distinctProductIds ?? throw new InvalidOperationException()));

                                    //replace the query
                                    nextOperation.DatabaseQuery = replacedQuery;
                                    return;
                                }

                            default:
                                {
                                    return;
                                }
                        }
                    });

            //get the orders
            var orders = 
                (result
                    .Skip(1)
                    .First() as AbstractSqlQueryResultOperation<Order>)
                ?.ComputedResult;

            //get the products
            var products =
                (result
                    .Skip(2)
                    .First() as AbstractSqlQueryResultOperation<Product>)
                ?.ComputedResult
                .ToDictionary(x => x.ProductId, x => x);

            //add in each order the ordered product
            foreach (var order in orders ?? new List<Order>())
            {
                order.OrderedProducts.Add(products?[order.ProductId]);
            }

            return orders;
        }

        public async Task<IList<Product>> GetAllProductsAsync()
        {
            //get the transaction builder
            var transactionBuilder = 
                GetTransactionBuilderForAllProducts();

            //get the result
            var result = 
                await _schedulingService
                    .ScheduleAndExecuteTransactionAsync(transactionBuilder.BuildTransaction());

            //get all the products
            return
                (result
                    .First() as AbstractSqlQueryResultOperation<Product>)
                ?.ComputedResult ?? new List<Product>();
        }

        public async Task<IList<Client>> GetAllClientsAsync()
        {
            //get transaction builder
            var transactionBuilder =
                GetTransactionBuilderForGetAllClients();

            //get the result
            var result =
                await _schedulingService
                    .ScheduleAndExecuteTransactionAsync(transactionBuilder.BuildTransaction());

            //get all the products
            return (result
                    .First() as AbstractSqlQueryResultOperation<Client>)
                ?.ComputedResult ?? new List<Client>();
        }
    }
}
