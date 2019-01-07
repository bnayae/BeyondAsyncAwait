using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

// TODO: TDF version

namespace Bnaya.Samples
{
    static class Program
    {
        private static Server _server;
        private static BlockingCollection<CorrelationItem<int>> _requestChannel;
        private static BlockingCollection<CorrelationItem<string>> _responseChannel;

        static void Main(string[] args)
        {
            _requestChannel = new BlockingCollection<CorrelationItem<int>>();
            _responseChannel = new BlockingCollection<CorrelationItem<string>>();
            _server = new Server(_requestChannel, _responseChannel);

            //TODO: remove the Dequeue loop
            Thread t = new Thread(DequeueLoop);
            t.Start();

            int[] requets = { 1, 5, 1, 3 };
            foreach (var r in requets) 
            {
                var item = new CorrelationItem<int>(r);
                Console.WriteLine($"Sending: {item.Value} [{item.Correlation:N}]");
                _requestChannel.Add(item);
                // TODO: replace _requestChannel.Add(item); with wrapper that return Task<string>
            }

            Console.ReadKey();
        }

        private static void DequeueLoop()
        {
            foreach (CorrelationItem<string> item in _responseChannel.GetConsumingEnumerable())
            {
                Console.WriteLine($"{item.Value} [{item.Correlation:N}]");
            }
        }
    }
}
