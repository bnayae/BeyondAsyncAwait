using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
            var responses = new List<Task<string>>();
            foreach (var r in requets) // TODO: unblocking calls
            {
                Console.WriteLine($"Sending: {r}");
                var item = new CorrelationItem<int>(r);
                Task<string> result = _rpc.SendAsync(r);
                responses.Add(result);
            }

            // ==================== Loop all Task (until completion) in parallel =======================


            char[] chars = { '.', '-', '~', '^' };
            for (int i = 0; i < responses.Count; i++)
            {
                char c = chars[i];
                Task<string> response = responses[i];
                var t = new Thread(() =>
                {
                    while (!response.IsCompleted) // loop single task until completion
                    {
                        Console.Write(c);
                        Thread.Sleep(100);
                    }
                    Console.WriteLine($" Result = {response.Result}");
                });
                t.Start();
            }

            #region Continue With

            //foreach (var r in requets) // TODO: unblocking calls
            //{
            //    Console.WriteLine($"Sending: {r}");
            //    var item = new CorrelationItem<int>(r);
            //    Task<string> t = _rpc.SendAsync(r);
            //    t.ContinueWith(c => Console.WriteLine(c.Result));
            //}

            #endregion // Continue With

            Console.ReadKey();
        }
    }
}
