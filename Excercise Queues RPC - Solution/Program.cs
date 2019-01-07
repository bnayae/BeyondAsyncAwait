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
        private static RpcBridge<int, string> _rpc;
        private static BlockingCollection<CorrelationItem<int>> _requestChannel;
        private static BlockingCollection<CorrelationItem<string>> _responseChannel;

        static void Main(string[] args)
        {
            _requestChannel = new BlockingCollection<CorrelationItem<int>>();
            _responseChannel = new BlockingCollection<CorrelationItem<string>>();
            _rpc = new RpcBridge<int, string>(_requestChannel, _responseChannel);
            _server = new Server(_requestChannel, _responseChannel);


            int[] requets = { 1, 5, 1, 3 };
            foreach (var r in requets) // TODO: unblocking calls
            {
                Console.WriteLine($"Sending: {r}");
                var item = new CorrelationItem<int>(r);
                Task<string> t = _rpc.SendAsync(r);
                t.ContinueWith(c => Console.WriteLine(c.Result));
            }
            //foreach (var r in requets) // TODO: unblocking calls
            //{
            //    Console.WriteLine($"Sending: {r}");
            //    var item = new CorrelationItem<int>(r);
            //    Task<string> result = _rpc.SendAsync(r);
            //    while (!result.IsCompleted)
            //    {
            //        Console.Write(".");
            //        Thread.Sleep(100);
            //    }
            //    Console.WriteLine($" Result = {result.Result}");
            //}

            Console.ReadKey();
        }
    }
}
