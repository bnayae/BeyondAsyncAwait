using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
#pragma warning disable CS0618 // Type or member is obsolete

namespace AopStyleApi
{
    class Program
    {
        static void Main(string[] args)
        {
            Task t = ExecuteAsync<int>(
                    Enumerable.Range(0, 10),
                    async i =>
                    {
                        Console.Write($"{i}, ");
                        await Task.Delay(500);
                        if (i == 4)
                            await Task.Delay(Timeout.InfiniteTimeSpan);
                        return;
                    }, 1);
            while (!t.IsCompleted)
            {
                Console.Write(".");
                Thread.Sleep(100);
            }

            Console.WriteLine("Done");
            Console.ReadKey();
        }

        private static Task ExecuteAsync<T>(IEnumerable<T> data, Func<T, Task> executer, int limit = 1)
        {
            Func<T, Task> aop = async item =>
            {
                try
                {
                    //await executer(item).WithTimeout(TimeSpan.FromSeconds(3));
                    if (await executer(item).IsTimeoutAsync(TimeSpan.FromSeconds(3)).ConfigureAwait(false))
                    {
                        Console.Write(" Timeout ");
                        return;
                    }
                    //Task exTask = executer(item);
                    //Task any = await Task.WhenAny(exTask, Task.Delay(3000));
                    //if (any != exTask)
                    //{
                    //    Console.Write(" Timeout ");
                    //    return;
                    //}
                }
                catch (TimeoutException)
                {
                    Console.Write(" Timeout ");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex}");
                }
            };
            var ab = new ActionBlock<T>(aop, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = limit });
            foreach (var item in data)
            {
                ab.Post(item);
            }
            ab.Complete();
            return ab.Completion;
        }
    }
}
