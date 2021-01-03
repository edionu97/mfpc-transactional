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

            var managementService = serviceRepo.ServiceProvider.GetRequiredService<IManagementService>();

            var schedulingService = serviceRepo.ServiceProvider.GetRequiredService<ISchedulingService>();


            var t1 = schedulingService.ScheduleAndExecuteTransactionAsync(new List<Tuple<Operation, Lock, int>>
            {
                Tuple.Create(new Operation()
                {
                    DatabaseQuery = "delete * from a",
                }, new Lock
                {
                    LockType = LockType.Write,
                    TableName = "a",
                    Object = "a"
                }, 0),

                Tuple.Create(new Operation()
                {
                    DatabaseQuery = "delete * from b",
                }, new Lock
                {
                    LockType = LockType.Read,
                    TableName = "b",
                    Object = "b"
                }, 10000)
            });

            var t3 = schedulingService.ScheduleAndExecuteTransactionAsync(new List<Tuple<Operation, Lock, int>>
            {
                Tuple.Create(new Operation()
                {
                    DatabaseQuery = "delete * from b",
                }, new Lock
                {
                    LockType = LockType.Write,
                        TableName = "b",
                    Object = "b"
                },0),
                Tuple.Create(new Operation()
                {
                    DatabaseQuery = "delete * from a",
                }, new Lock
                {
                    LockType = LockType.Read,
                    TableName = "a",
                    Object = "a"
                }, 1000)
            });

            var t2 = Task.Run(async () =>
            {
                await Task.Delay(10000);
            });

            Task.WaitAll(t1, t2, t3);

            //foreach (var el in await managementService.GetAllTransactionsAsync())
            //{
            //    await managementService.RemoveTransactionAsync(el);
            //}

            Console.ReadKey();

        }
    }
}
