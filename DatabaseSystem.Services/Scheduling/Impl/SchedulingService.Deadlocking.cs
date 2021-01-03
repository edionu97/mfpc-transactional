﻿using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using DatabaseSystem.Transactional.Graph.Element;
using DatabaseSystem.Transactional.Graph.Impl;
using DatabaseSystem.Transactional.Transactional;
using DatabaseSystem.Utility.ExtensionMethods;

namespace DatabaseSystem.Services.Scheduling.Impl
{
    public partial class SchedulingService
    {
        private volatile ConcurrentDictionary<int, IGraphElement> _transactionWaitingTask =
            new ConcurrentDictionary<int, IGraphElement>();

        private async Task FindDeadlocks()
        {
            var random = new Random();
            while (true)
            {
                //sleep 
                await Task.Delay(1000);

                //get cycle in graph
                var cycle = _graph.GetACycle();
                if (!cycle.Any())
                {
                    continue;
                }

                //if we know all the threads that are running the waiting tasks
                if (cycle.Any(x => !_transactionWaitingTask.ContainsKey(x.Id)))
                {
                    continue;
                }

                var canceledTaskIdx = cycle[random.Next(cycle.Count)].Id;

                //get the random transaction and cancel it
                if (_transactionWaitingTask.TryGetValue(canceledTaskIdx, out var waitingTask))
                {
                    (waitingTask as TransactionalGraphElement)?
                        .CancellationTokenSource
                        .Cancel();
                };

                Console.WriteLine($"Found {cycle.Count}");
            }

            // ReSharper disable once FunctionNeverReturns
        }

        private Task RemoveInactiveTasksFromDictionary()
        {
            return Task.Run(() =>
            {
                foreach (var key in _transactionWaitingTask.Keys.ToList())
                {
                    //get the value
                    IGraphElement transactionInfo;
                    while (!_transactionWaitingTask.TryGetValue(key, out transactionInfo))
                    {
                    }

                    //get the task
                    var runningTask = (transactionInfo as TransactionalGraphElement)?.ActiveTask;
                    if (runningTask?.Status.IsActiveStatus() == true)
                    {
                        continue;
                    }

                    //remove the key
                    while (!_transactionWaitingTask.TryRemove(key, out _))
                    {
                    }
                }
            });

        }
    }
}
