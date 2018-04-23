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
        private static Timer _tmr;

        #region Main

        static void Main(string[] args)
        {
            Console.WriteLine("Starting");

            #region Setup Stage Listeners

            // start the second message processing loop
            _tmr = new Timer(SecondStageAsync, null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));

            #endregion // Setup Stage Listeners

            // start sending messages
            Task<string> t = FirstStageAsync("Single Call");

            // hangout until all messages completes
            while (!t.IsCompleted)
            {
                Console.Write(".");
                Thread.Sleep(100);
            }

            Console.WriteLine($"\r\nDone: {t.Result}");
            Console.ReadKey(true);
        }

        #endregion // Main

        #region FirstStageAsync > _firstQueue

        private static /*async*/ Task<string> FirstStageAsync(string input)
        {
            string data = $"{input} -> first stage";

            var message = Completeble.Create(data);
            _firstQueue.Enqueue(message); // enqueue and complete
            Console.Write(" 1st ");
            return message.Task;
            //string result = await message.Task;  // this is the magic
            //Console.WriteLine($"\r\n{result}");
            //return result;
        }

        #endregion // FirstStageAsync > _firstQueue

        #region _firstQueue > SecondStageAsync > _secondQueue

        private static void SecondStageAsync(object state)
        {
            while (_firstQueue.TryDequeue(out Completeble<string> message))
            {
                string data = $"{message.Value} -> second stage";
                Console.Write(" 2nd ");

                var nextMessage = message.TryComplete(data);
            }
        }

        #endregion // _firstQueue > SecondStageAsync > _secondQueue
    }
}