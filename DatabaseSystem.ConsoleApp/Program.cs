using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using DatabaseSystem.ConsoleApp.Config;
using DatabaseSystem.Persistence.Models;
using DatabaseSystem.Services.Scheduling;
using DatabaseSystem.Services.SqlExecutor;
using DatabaseSystem.Services.SqlExecutor.SqlOperations;
using DatabaseSystem.Utility.Attributes;
using DatabaseSystem.Utility.Enums;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;

namespace DatabaseSystem.ConsoleApp
{
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
        public static  async Task Main(string[] args)
        {
            var host = Bootstrapper.Load();

            using var serviceRepo = host.Services.CreateScope();

            var schedulingService = serviceRepo.ServiceProvider.GetRequiredService<ISchedulingService>();
            var sqlExecutor = serviceRepo.ServiceProvider.GetRequiredService<ISqlExecutorService>();

            //get the value of the first column (the id column)
            var aId = (int) await sqlExecutor.ExecuteScalarAsync("select * from a;");
            var bId = (int) await sqlExecutor.ExecuteScalarAsync("select * from b;");
            Console.WriteLine($"AId = {aId}, BId = {bId}");


            var firstTransaction = schedulingService.ScheduleAndExecuteTransactionAsync(new List<Tuple<Operation, Lock, int?>>
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
                            Value = aId
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

            var secondTransaction = schedulingService.ScheduleAndExecuteTransactionAsync(new List<Tuple<Operation, Lock, int?>>
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
                            Value = bId
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

            try
            {
                Task.WaitAll(firstTransaction, secondTransaction);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
