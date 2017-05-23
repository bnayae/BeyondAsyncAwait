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
        private static ConcurrentQueue<Completeble<string>> _firstQueue = new ConcurrentQueue<Completeble<string>>();
        private static BlockingCollection<Completeble<string>> _secondQueue = new BlockingCollection<Completeble<string>>();
        private static ActionBlock<Completeble<string>> _finalProcesing;
        private static Timer _tmr;

        #region Main

        static void Main(string[] args)
        {
            Console.WriteLine("Starting");

            #region Setup Stage Listeners

            // start the second message processing loop
            _tmr = new Timer(SecondStageAsync, null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));
            // start the third message processing loop
            Task _ = FinalStageAsync(); 
            // define the last message processing handler
            _finalProcesing = new ActionBlock<Completeble<string>>(ProcessFinalStageAsync);

            #endregion // Setup Stage Listeners

            // start sending messages
            Task<string[]> t = StartAsync();

            // hangout until all messages completes
            while (!t.IsCompleted)
            {
                Console.Write(".");
                Thread.Sleep(100);
            }
            Console.WriteLine($"\r\nAll Task completes [{t.Result.Length}] messages");
            _secondQueue.CompleteAdding();
            Console.ReadKey(true);
        }

        #endregion // Main

        #region StartAsync

        private static Task<string[]> StartAsync()
        {
            var tasks = from i in Enumerable.Range(1, 3)
                        select FirstStageAsync($"Message in a bottle {i}");
            return Task.WhenAll(tasks);
        }

        #endregion // StartAsync

        #region FirstStageAsync

        private static async Task<string> FirstStageAsync(string input)
        {
            string data = $"{input} -> first stage";

            var message = Completeble.Create(data);
            _firstQueue.Enqueue(message); // enqueue and complete
            Console.Write(" 1st ");
            string result = await message.Task;  // this is the magic
            Console.WriteLine($"\r\n{result}");
            return result;
        }

        #endregion // FirstStageAsync

        #region SecondStageAsync

        private static void SecondStageAsync(object state)
        {
            Completeble<string> message;
            while (_firstQueue.TryDequeue(out message))
            {
                string data = $"{message.Value} -> second stage";

                var nextMessage = message.ProceedWith(data);
                _secondQueue.Add(nextMessage); // enqueue and complete
                Console.Write(" 2nd ");
            }
        }

        #endregion // SecondStageAsync

        #region FinalStageAsync

        private static async Task FinalStageAsync()
        {
            // make the rest of the code async 
            // otherwise you will have dead lock (GetConsumingEnumerable is 
            // blocking API therefore could be dangerous)
            await Task.Delay(1);  

            foreach (Completeble<string> message in _secondQueue.GetConsumingEnumerable())
            {
                await Task.Delay(2000);

                string data = $"{message.Value} -> third stage";

                var nextMessage = message.ProceedWith(data);
                _finalProcesing.Post(nextMessage); // enqueue and complete
                Console.Write(" 3rd ");
            }
        }

        #endregion // FinalStageAsync

        #region ProcessFinalStageAsync

        private static async Task ProcessFinalStageAsync(Completeble<string> message)
        {
            await Task.Delay(1000);

            string next = $"{message.Value} -> last stage";

            var nextMessage = message.ProceedWith(next);
            if (!nextMessage.TryComplete())
                Console.WriteLine("Completion failed (can only complete once)");
        }

        #endregion // ProcessFinalStageAsync
    }
}