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
        private static RpcBridge _rpc;

        static void Main(string[] args)
        {
            _rpc = new RpcBridge();
            _server = new Server();

            int[] requets = { 1, 5, 1, 3 };
            char[] chars = { '.', '-', '~', '^' };
            //Complex(requets, chars);

            Continuation(requets);

            Console.ReadKey();
        }

        private static void Complex(int[] requets, char[] chars)
        {
            var responses = new List<Task<string>>();
            foreach (var r in requets) // TODO: unblocking calls
            {
                Console.WriteLine($"Sending: {r}");
                Task<string> result = _rpc.SendAsync(r);
                responses.Add(result);
            }

            // ==================== Loop all Task (until completion) in parallel =======================


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
        }

        private static void Continuation(int[] requets)
        {
            foreach (var r in requets) // TODO: unblocking calls
            {
                Console.WriteLine($"Sending: {r}");
                Task<string> t = _rpc.SendAsync(r);
                t.ContinueWith(c => Console.WriteLine(c.Result));
            }

            while (true)
            {
                Console.Write(".");
                Thread.Sleep(100);
            }
        }
    }
}
