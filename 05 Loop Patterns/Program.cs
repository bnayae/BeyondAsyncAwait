using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using static System.Math;
#pragma warning disable Await1 // Method is not configured to be awaited

namespace Bnaya.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            #region Sequential (for loop)

            //Console.WriteLine("Start Sequential");
            //Task t = SequentialAsync(10);
            //while (!t.IsCompleted)
            //{
            //    Console.Write(".");
            //    Thread.Sleep(50);
            //}

            #endregion // Sequential

            Console.WriteLine("\r\n---------------------------------------------");

            #region Non-Sequential (LINQ of Tasks)

            //Console.WriteLine("Start Non-Sequential");
            //Task t1 = NonSequentialAsync(10);
            //while (!t1.IsCompleted)
            //{
            //    Console.Write(".");
            //    Thread.Sleep(50);
            //}

            #endregion // Non-Sequential

            //Console.WriteLine("\r\n---------------------------------------------");

            #region Sequential with Tdf

            //Console.WriteLine("Start Sequential over TPL Dataflow");
            //Task t2 = SequentialWithTdfAsync(10);
            //while (!t2.IsCompleted)
            //{
            //    Console.Write(".");
            //    Thread.Sleep(50);
            //}

            #endregion // Sequential with Tdf

            Console.WriteLine("\r\n---------------------------------------------");

            #region Non-Sequential with Tdf

            //Console.WriteLine("Start Non-Sequential over TPL Dataflow");
            //Task t3 = NonSequentialWithTdfAsync(4);
            //while (!t3.IsCompleted)
            //{
            //    Console.Write(".");
            //    Thread.Sleep(50);
            //}

            #endregion // Non-Sequential with Tdf

            #region CompleteWhenN

            Console.WriteLine("complete when n");
            Task t4 = CompleteWhenN();
            while (!t4.IsCompleted)
            {
                Console.Write(".");
                Thread.Sleep(50);
            }

            #endregion // CompleteWhenN

            Console.ReadKey(true);
        }

        #region SequentialAsync

        private static async Task SequentialAsync(int n)
        {
            for (int i = 0; i < n; i++)
            {
                await SingleStepAsync(i).ConfigureAwait(false);
            }
        }

        #endregion // SequentialAsync

        #region NonSequentialAsync

        private static Task NonSequentialAsync(int n)
        {
            var tasks = from i in Enumerable.Range(0, n)
                        select SingleStepAsync(i);
            return Task.WhenAll(tasks);
        }

        #endregion // NonSequentialAsync

        #region SequentialWithTdfAsync

        private static Task SequentialWithTdfAsync(int n)
        {
            var abSequential = new ActionBlock<int>(SingleStepAsync);
            for (int i = 0; i < 10; i++)
            {
                abSequential.Post(i);
            }
            abSequential.Complete();
            return abSequential.Completion;
        }

        #endregion // SequentialWithTdfAsync

        #region NonSequentialWithTdfAsync

        private static Task NonSequentialWithTdfAsync(int n)
        {
            var options = new ExecutionDataflowBlockOptions
                {
                    MaxDegreeOfParallelism = n
                };
            var abSequential = new ActionBlock<int>(SingleStepAsync,
                                                    options);
            for (int i = 0; i < 10; i++)
            {
                abSequential.Post(i);
            }
            abSequential.Complete();
            return abSequential.Completion;
        }

        #endregion // NonSequentialWithTdfAsync

        #region CompleteWhenN

        private static async Task CompleteWhenN()
        {
            var cts = new CancellationTokenSource();
            var tasks = from i in Enumerable.Range(0, 20)
                        select SingleStepWithResultAsync(i, cts.Token);
            await tasks.When(i => i > 4 && i < 8); // more common to use it with Task<bool>
            //await tasks.WhenN(2);
            //await tasks.WhenN(2, i => i % 2 == 0);
            //await tasks.WhenN(4, cts); // cancel signaling (to none completed tasks)
            Console.WriteLine("COMPLETE");
        }

        #endregion // CompleteWhenN

        #region SingleStepAsync / SingleStepWithResultAsync

        private static async Task<int> SingleStepWithResultAsync(
                                int i,
                                CancellationToken cancellationToken)
        {
            #region ThrowIfCancellationRequested

            //cancellationToken.ThrowIfCancellationRequested();
            if (cancellationToken.IsCancellationRequested)
            {
                Console.Write("X");
                throw new OperationCanceledException();
            }

            #endregion // ThrowIfCancellationRequested
            int delay = Abs(3000 - (i * 100));

            await Task.Delay(delay);
            Console.Write($"{i}, ");
            return i;
        }

        private static async Task SingleStepAsync(int i)
        {
            int delay = Abs(3000 - (i * 100));

            await Task.Delay(delay);
            Console.Write($"{i}, ");
        }

        #endregion // SingleStepAsync / SingleStepWithResultAsync
    }
}