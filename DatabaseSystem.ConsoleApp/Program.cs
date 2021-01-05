using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Channels;
using System.Threading.Tasks;
using DatabaseSystem.ConsoleApp.Config;
using DatabaseSystem.Persistence.Models;
using DatabaseSystem.Services.Scheduling;
using DatabaseSystem.Services.SqlExecutor.SqlOperations;
using DatabaseSystem.Utility.Attributes;
using DatabaseSystem.Utility.Enums;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using OnlineShopping.Services;

namespace DatabaseSystem.ConsoleApp
{
    public class Client
    {
        [Map("clientId", SqlDbType.Int, true)]
        public int Id { get; set; }

        [Map("clientName", SqlDbType.VarChar)]
        public string Name { get; set; }

        [Map("phone", SqlDbType.VarChar)]
        public string Phone { get; set; }

        [Map("emailAddress", SqlDbType.VarChar)]
        public string Email { get; set; }
    }

    class A
    {
        [Map("aId", SqlDbType.Int, true)]
        public int Id { get; set; }

        [Map("val", SqlDbType.Int)]
        public int Val { get; set; }
    }

    class B
    {
        [Map("bId", SqlDbType.Int, true)]
        public int Id { get; set; }

        [Map("val", SqlDbType.Int)]
        public int Val { get; set; }
    }

    public class Program
    {
        public static async Task Main(string[] args)
        {

            var host = Bootstrapper.Load();
            using var serviceRepo = host.Services.CreateScope();


            //get the shopping service
            var shoppingService = serviceRepo.ServiceProvider.GetRequiredService<IShoppingService>();

            //await shoppingService.RegisterNewClientAsync(
            //"ciucanu",
            //"sorina",
            //"22345678901",
            //"0751349573",
            //"soriina20@gmail.com");

            //await shoppingService.AddProductAsync("paste", 4, "barila", 1012);

            // await shoppingService.AddOrderAsync(3, 2, 2);

            //await shoppingService.GetOrdersForClientAsync("1970114270015");

            try
            {
                await shoppingService.AddOrderAsync(2, 2, 1);
                await shoppingService.AddOrderAsync(2, 3, 1);
                await shoppingService.AddOrderAsync(2, 6, 1);


                await shoppingService.AddOrderAsync(3, 6, 10);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

         

            var orders = await shoppingService.GetOrdersForClientAsync("1970114270015");





            if (1 == 1)
            {
                return;
            }

            var schedulingService = serviceRepo.ServiceProvider.GetRequiredService<ISchedulingService>();
            //var rez = await schedulingService.ScheduleAndExecuteTransactionAsync(new List<Tuple<Operation, Lock, int?>>
            //{
            //    Tuple.Create<Operation, Lock, int?>(new DeleteOperation<Client>
            //        {
            //            SelectExistingDataQuery = "select top(1) * from client where clientId = @clientId",
            //            //DatabaseQuery = "update client set clientName = @clientName, phone = @phone where clientId = @clientId",
            //            DatabaseQuery = "delete from client where clientId = @clientId",
            //            UndoDatabaseQuery = "insert into client values(@clientName, @phone, @emailAddress)",
            //            CommandParameters = new List<SqlParameter>
            //            {
            //                new SqlParameter("@clientId", SqlDbType.Int)
            //                {
            //                    Value = 3
            //                },
            //            }
            //        },
            //        new Lock
            //        {
            //            LockType = LockType.Write,
            //            TableName = "client",
            //            Object = "client"
            //        }, 0)
            //});

            ////var selectResult = (rez.First() as AbstractSqlQueryResultOperation<Client>)?.ComputedResult;

            //if (1 == 1)
            //{
            //    return;
            //}


            var t1 = schedulingService.ScheduleAndExecuteTransactionAsync(new List<Tuple<Operation, Lock, int?>>
            {
                Tuple.Create<Operation, Lock, int?>(new DeleteOperation<A>()
                {
                    DatabaseQuery = "delete from a where aId = @aId",
                    UndoDatabaseQuery = "insert into a values (@val)",
                    SelectExistingDataQuery = "select top(1) * from a where aId = @aId",
                    CommandParameters = new List<SqlParameter>
                    {
                        new SqlParameter("@aId", SqlDbType.Int)
                        {
                            Value = 12
                        }
                    }

                }, new Lock
                {
                    LockType = LockType.Write,
                    TableName = "a",
                    Object = "a"
                }, 0),

                Tuple.Create<Operation, Lock, int?>(new SelectOperation<B>()
                {
                    DatabaseQuery = "select * from b",
                }, new Lock
                {
                    LockType = LockType.Read,
                    TableName = "b",
                    Object = "b"
                }, 10000)
            });

            await t1;

            var t3 = schedulingService.ScheduleAndExecuteTransactionAsync(new List<Tuple<Operation, Lock, int?>>
            {
                Tuple.Create<Operation, Lock, int?>(new DeleteOperation<B>()
                {
                    DatabaseQuery = "delete from b where bId = @bId",
                    SelectExistingDataQuery = "select top(1) * from b where bId = @bId",
                    UndoDatabaseQuery = "insert into b values (@val)",
                    CommandParameters = new List<SqlParameter>
                    {
                        new SqlParameter("@bId", SqlDbType.Int)
                        {
                            Value = 10
                        }
                    }
                }, new Lock
                {
                    LockType = LockType.Write,
                    TableName = "b",
                    Object = "b"
                },0),
                Tuple.Create<Operation, Lock, int?>(new SelectOperation<A>()
                {
                    DatabaseQuery = "select * from a",
                }, new Lock
                {
                    LockType = LockType.Read,
                    TableName = "a",
                    Object = "a"
                }, 1000)
            });

            await t3;

            try
            {
                Task.WaitAll(t1, t3);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
