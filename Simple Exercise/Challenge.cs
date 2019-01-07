using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

// IMPORTANT: this file (Challenge.cs) properties Build Action is set to None
//            Set it to Compile before trying to build or debug this project
//            (right click -> properties)
namespace Exercise
{
    public static class Challenge
    {
        private static BlockingCollection</* encapsulation of future completed task */> _queue =
            new BlockingCollection</* encapsulation of future completed task */>();


        private static void Start()
        {
            Task.Factory.StartNew(() =>
            {
                foreach (var item in _queue.GetConsumingEnumerable())
                {
                    Thread.Sleep(1000); // log running task (not using a thread-pool's thread)
                    // TODO: set the task encapsulation into completion state
                }
            }, TaskCreationOptions.LongRunning);

            string[] arr = { "A", "B", "C" };
            Task t = ProcessSequentiallyAsync(arr);
            while (!t.IsCompleted)
            {
                Console.Write(".");
                Thread.Sleep(50);
            }
        }

        private static async Task ProcessSequentiallyAsync(string[] items)
        {
            // TODO: call ProcessAsync(...) sequentially for each item (one ofter the completion of the other)
        }

        private static async Task ProcessAsync(string data)
        {
            Console.Write(data);
            // enqueue the data (don't forget to encapsulate it with semantic of task)

            // result = await the completion of the data processing
            Console.Write(result);
        }
    }
}
