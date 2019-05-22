using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using static Bnaya.Samples.Storage;

// TODO: TDF version

namespace Bnaya.Samples
{
    static class Program
    {
        private static Server _server;

        static void Main(string[] args)
        {
            _server = new Server();

            //TODO: remove the Dequeue loop
            Thread t = new Thread(DequeueLoop);
            t.Start();

            int[] requets = { 1, 5, 1, 3 };
            foreach (var r in requets) 
            {
                var item = new CorrelationItem<int>(r);
                Console.WriteLine($"Sending: {item.Value} [{item.Correlation:N}]");
                RequestChannel.Add(item);
                // TODO: replace _requestChannel.Add(item); with wrapper that return Task<string>
            }

            Console.ReadKey();
        }

        private static void DequeueLoop()
        {
            foreach (CorrelationItem<string> item in ResponseChannel.GetConsumingEnumerable())
            {
                Console.WriteLine($"{item.Value} [{item.Correlation:N}]");
            }
        }
    }
}
