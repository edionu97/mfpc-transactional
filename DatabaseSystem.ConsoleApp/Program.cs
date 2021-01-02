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

            
            var t1 =  schedulingService.ScheduleAndExecuteTransactionAsync(new List<Tuple<Operation, Lock>>
            {
                Tuple.Create(new Operation()
                {
                    DatabaseQuery = "delete * from a",
                }, new Lock
                {
                    LockType = LockType.Write,
                    TableName = "a",
                    Object = "a"
                })
            });


            var t3 = schedulingService.ScheduleAndExecuteTransactionAsync(new List<Tuple<Operation, Lock>>
            {
                Tuple.Create(new Operation()
                {
                    DatabaseQuery = "delete * from a",
                }, new Lock
                {
                    LockType = LockType.Write,
                    TableName = "a",
                    Object = "a"
                })
            });

            var t2 = Task.Run(async () =>
            {
                await Task.Delay(10000);

                await managementService.ReleaseLockAsync(new Lock()
                {
                    LockId = 16
                });

                await managementService.ReleaseLockAsync(new Lock()
                {
                    LockId = 17
                });
            });

            Task.WaitAll(t1, t2, t3);

            //foreach (var el in await managementService.GetAllTransactionsAsync())
            //{
            //    await managementService.RemoveTransactionAsync(el);
            //}

        }
    }
}
