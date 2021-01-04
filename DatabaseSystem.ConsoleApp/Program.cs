using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using DatabaseSystem.ConsoleApp.Config;
using DatabaseSystem.Persistence.Models;
using DatabaseSystem.Services.Scheduling;
using DatabaseSystem.Services.SqlExecutor.SqlOperations;
using DatabaseSystem.Utility.Attributes;
using DatabaseSystem.Utility.Enums;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;

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

    public class Program
    {
        public static async Task Main(string[] args)
        {

            var host = Bootstrapper.Load();

            using var serviceRepo = host.Services.CreateScope();

            var schedulingService = serviceRepo.ServiceProvider.GetRequiredService<ISchedulingService>();

            var rez = await schedulingService.ScheduleAndExecuteTransactionAsync(new List<Tuple<Operation, Lock, int?>>
            {
                Tuple.Create<Operation, Lock, int?>(new DeleteOperation<Client>
                    {
                        SelectExistingDataQuery = "select top(1) * from client where clientId = @clientId",
                        //DatabaseQuery = "update client set clientName = @clientName, phone = @phone where clientId = @clientId",
                        DatabaseQuery = "delete from client where clientId = @clientId",
                        UndoDatabaseQuery = "insert into client values(@clientName, @phone, @emailAddress)",
                        CommandParameters = new List<SqlParameter>
                        {
                            new SqlParameter("@clientId", SqlDbType.Int)
                            {
                                Value = 3
                            },
                        }
                    },
                    new Lock
                    {
                        LockType = LockType.Write,
                        TableName = "client",
                        Object = "client"
                    }, 0)
            });

            //var selectResult = (rez.First() as AbstractSqlQueryResultOperation<Client>)?.ComputedResult;

            if (1 == 1)
            {
                return;
            }


            var t1 = schedulingService.ScheduleAndExecuteTransactionAsync(new List<Tuple<Operation, Lock, int?>>
            {
                Tuple.Create<Operation, Lock, int?>(new Operation
                {
                    DatabaseQuery = "delete from a where 1 = 1",
                }, new Lock
                {
                    LockType = LockType.Write,
                    TableName = "a",
                    Object = "a"
                }, 0),

                Tuple.Create<Operation, Lock, int?>(new Operation()
                {
                    DatabaseQuery = "select * from b",
                }, new Lock
                {
                    LockType = LockType.Read,
                    TableName = "b",
                    Object = "b"
                }, 10000)
            });

            var t3 = schedulingService.ScheduleAndExecuteTransactionAsync(new List<Tuple<Operation, Lock, int?>>
            {
                Tuple.Create<Operation, Lock, int?>(new Operation()
                {
                    DatabaseQuery = "delete from b where 1 = 1",
                }, new Lock
                {
                    LockType = LockType.Write,
                        TableName = "b",
                    Object = "b"
                },0),
                Tuple.Create<Operation, Lock, int?>(new Operation()
                {
                    DatabaseQuery = "select * from a",
                }, new Lock
                {
                    LockType = LockType.Read,
                    TableName = "a",
                    Object = "a"
                }, 1000)
            });

            var t2 = schedulingService.ScheduleAndExecuteTransactionAsync(new List<Tuple<Operation, Lock, int?>>
            {
                //op0
                Tuple.Create<Operation, Lock, int?>(new Operation()
                {
                    DatabaseQuery = "select * from c",
                }, new Lock
                {
                    LockType = LockType.Read,
                    TableName = "c",
                    Object = "c"
                }, 500),

                //op1
                Tuple.Create<Operation, Lock, int?>(new Operation()
                {
                    DatabaseQuery = "select * from c",
                }, new Lock
                {
                    LockType = LockType.Read,
                    TableName = "c",
                    Object = "c"
                }, 500),

                //op2
                Tuple.Create<Operation, Lock, int?>(new Operation()
                {
                    DatabaseQuery = "delete * from c",
                }, new Lock
                {
                    LockType = LockType.Write,
                    TableName = "c",
                    Object = "c"
                }, 500)

            });

            Task.WaitAll(t1, t2, t3);



        }
    }
}
