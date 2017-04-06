using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Bnaya.Samples
{
    class Program
    {
        private static ConcurrentQueue<Completeble<string>> _q1 = new ConcurrentQueue<Completeble<string>>();
        private static BlockingCollection<Completeble<string>> _q2 = new BlockingCollection<Completeble<string>>();
        private static ActionBlock<Completeble<string>> _actionBlock;
        private static Timer _tmr;

        static void Main(string[] args)
        {
            Console.WriteLine("Starting");

            // start the second message processing loop
            _tmr = new Timer(SecondStageAsync, null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));
            // start the third message processing loop
            Task _ = ThirdStageAsync(); 
            // define the last message processing handler
            _actionBlock = new ActionBlock<Completeble<string>>(LastStageAsync);

            // start sending messages
            Task<string[]> t = StartAsync();
            // hangout until all messages completes
            while (!t.IsCompleted)
            {
                Console.Write(".");
                Thread.Sleep(100);
            }
            Console.WriteLine($"\r\nAll Task completes [{t.Result.Length}] messages");
            _q2.CompleteAdding();
            Console.ReadKey(true);
        }

        private static Task<string[]> StartAsync()
        {
            var tasks = from i in Enumerable.Range(1, 3)
                        select FirstStageAsync($"Message in a bottle {i}");
            return Task.WhenAll(tasks);
        }

        private static async Task<string> FirstStageAsync(string input)
        {
            string data = $"{input} -> first stage";

            var message = Completeble.Create(data);
            _q1.Enqueue(message); // enqueue and complete
            Console.Write(" 1st ");
            string result = await message.Task;  // this is the magic
            Console.WriteLine($"\r\n{result}");
            return result;
        }

        private static void SecondStageAsync(object state)
        {
            Completeble<string> message;
            while (_q1.TryDequeue(out message))
            {
                string data = $"{message.Value} -> second stage";

                var nextMessage = message.ProceedWith(data);
                _q2.Add(nextMessage); // enqueue and complete
                Console.Write(" 2nd ");
            }
        }

        private static async Task ThirdStageAsync()
        {
            // make the rest of the code async 
            // otherwise you will have dead lock (GetConsumingEnumerable is 
            // blocking API therefore could be dangerous)
            await Task.Delay(1);  

            foreach (Completeble<string> message in _q2.GetConsumingEnumerable())
            {
                await Task.Delay(2000);

                string data = $"{message.Value} -> third stage";

                var nextMessage = message.ProceedWith(data);
                _actionBlock.Post(nextMessage); // enqueue and complete
                Console.Write(" 3rd ");
            }
        }

        private static async Task LastStageAsync(Completeble<string> message)
        {
            await Task.Delay(1000);

            string next = $"{message.Value} -> last stage";

            var nextMessage = message.ProceedWith(next);
            if (!nextMessage.TryComplete())
                Console.WriteLine("Completion failed (can only complete once)");
        }
    }
}