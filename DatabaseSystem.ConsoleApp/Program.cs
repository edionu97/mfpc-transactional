using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DatabaseSystem.ConsoleApp.Config;
using DatabaseSystem.Persistence.Models;
using DatabaseSystem.Services.Management;
using DatabaseSystem.Services.Scheduling;
using DatabaseSystem.Utility.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace DatabaseSystem.ConsoleApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {

            var host = Bootstrapper.Load();

            using var serviceRepo = host.Services.CreateScope();

            var schedulingService = serviceRepo.ServiceProvider.GetRequiredService<ISchedulingService>();


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
