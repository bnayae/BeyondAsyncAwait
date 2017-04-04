using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Bnaya.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start Sequential");
            Task t = SequentialAsync(10);
            while (!t.IsCompleted)
            {
                Console.Write(".");
                Thread.Sleep(50);
            }

            Console.WriteLine("\r\n---------------------------------------------");

            Console.WriteLine("Start Non-Sequential");
            Task t1 = NonSequentialAsync(10);
            while (!t1.IsCompleted)
            {
                Console.Write(".");
                Thread.Sleep(50);
            }

            Console.WriteLine("\r\n---------------------------------------------");

            Console.WriteLine("Start Sequential over TPL Dataflow");
            Task t2 = SequentialWithTdfAsync(10);
            while (!t2.IsCompleted)
            {
                Console.Write(".");
                Thread.Sleep(50);
            }

            Console.WriteLine("\r\n---------------------------------------------");

            Console.WriteLine("Start Non-Sequential over TPL Dataflow");
            Task t3 = NonSequentialWithTdfAsync(10);
            while (!t3.IsCompleted)
            {
                Console.Write(".");
                Thread.Sleep(50);
            }

            Console.ReadKey(true);
        }

        private static async Task SequentialAsync(int n)
        {
            for (int i = 0; i < n; i++)
            {
                await SingleStepAsync(i);
            }
        }

        private static async Task NonSequentialAsync(int n)
        {
            var tasks = from i in Enumerable.Range(0, n)
                        select SingleStepAsync(i);
            await Task.WhenAll(tasks);
        }

        private static async Task SequentialWithTdfAsync(int n)
        {
            var abSequential = new ActionBlock<int>(SingleStepAsync);
            for (int i = 0; i < 10; i++)
            {
                abSequential.Post(i);
            }
            abSequential.Complete();
            await abSequential.Completion;
        }

        private static async Task NonSequentialWithTdfAsync(int n)
        {
            var abSequential = new ActionBlock<int>(SingleStepAsync, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = n });
            for (int i = 0; i < 10; i++)
            {
                abSequential.Post(i);
            }
            abSequential.Complete();
            await abSequential.Completion;
        }

        private static async Task SingleStepAsync(int i)
        {
            await Task.Delay(500);
            Console.Write($"{i}, ");
        }
    }
}