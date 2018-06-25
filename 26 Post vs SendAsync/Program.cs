using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace _26_Post_vs_SendAsync
{
    class Program
    {
        private static ActionBlock<int> _processor;

        static void Main(string[] args)
        {
            Console.WriteLine("Check Memory Allocation");

            Feed();

            //Task t = FeedAsync();
            //t.Wait();

            Console.ReadKey();
        }

        private static async Task ProcessAsync(int i)
        {
            Console.Write($" {i} ");
            await Task.Delay(1200).ConfigureAwait(false);
            Console.Write(".");
        }

        private static void Feed()
        {
            _processor = new ActionBlock<int>(ProcessAsync,
                                    new ExecutionDataflowBlockOptions
                                    {
                                        BoundedCapacity = 5,
                                        //MaxDegreeOfParallelism = 3
                                    });
            foreach (var item in QueueProvider())
            {
                if(!_processor.Post(item))
                    Console.Write(" Drop");
            }
        }

        private static async Task FeedAsync()
        {
            _processor = new ActionBlock<int>(ProcessAsync,
                                    new ExecutionDataflowBlockOptions
                                            {
                                                BoundedCapacity = 3 * 2 ,
                                                MaxDegreeOfParallelism = 3
                                            }); // can be correlate to the degree of parallelism
            foreach (var item in QueueProvider())
            {
                if (!await _processor.SendAsync(item).ConfigureAwait(false))
                    Console.Write(" Drop");
                Console.Write("#");
            }
        }

        private static IEnumerable<int> QueueProvider()
        {
            int i = 0;
            while (true)
            {
                yield return i++;
                if (i == int.MaxValue)
                    i = 0;
            }
        }
    }
}
