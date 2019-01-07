using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Exercise
{
    class Program
    {
        // using C# 7 Value Tuple
        private static BlockingCollection<(string Data, TaskCompletionSource<string> SemanticTask)> _queue =
            new BlockingCollection<(string, TaskCompletionSource<string>)>();

        static void Main(string[] args)
        {
            Task.Factory.StartNew(() =>
            {
                foreach (var item in _queue.GetConsumingEnumerable())
                {
                    Thread.Sleep(1000); // log running task (not using a thread-pool's thread)
                    // TODO: set the task encapsulation into completion state
                    item.SemanticTask.TrySetResult($"{item.Data}#");
                }
            }, TaskCreationOptions.LongRunning);

            string[] arr = { "A", "B", "C" };
            Task t = ProcessSequentiallyAsync(arr);
            while (!t.IsCompleted)
            {
                Console.Write(".");
                Thread.Sleep(50);
            }
            Console.ReadKey();
        }

        private static async Task ProcessSequentiallyAsync(string[] items)
        {
            // TODO: call ProcessAsync(...) sequentially for each item (one ofter the completion of the other)
            foreach (var data in items)
            {
                await ProcessAsync(data).ConfigureAwait(false);
            }
        }

        private static async Task ProcessAsync(string data)
        {
            Console.Write(data);
            // enqueue the data (don't forget to encapsulate it with semantic of task)
            var tcs = new TaskCompletionSource<string>();
            _queue.Add((data, tcs));
            // await the completion of the data processing
            var result = await tcs.Task.ConfigureAwait(false);
            Console.Write(result);
        }
    }
}
