using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Exercise
{
    class Program
    {
        private static BlockingCollection<KeyValuePair<Guid, string>> _queue =
            new BlockingCollection<KeyValuePair<Guid, string>>();

        private static ConcurrentDictionary<Guid, TaskCompletionSource<string>> _map =
            new ConcurrentDictionary<Guid, TaskCompletionSource<string>>();

        static void Main(string[] args)
        {
            Task.Factory.StartNew(() =>
            {
                foreach (var item in _queue.GetConsumingEnumerable())
                {
                    // TODO: task the value from dictionary and make it complete
                    TaskCompletionSource<string> tcs;
                    if (_map.TryRemove(item.Key, out tcs))
                        tcs.TrySetResult($"# {item.Value}");
                }
            }, TaskCreationOptions.LongRunning);

            Task _ = RunAsync();
            Console.ReadKey();
        }

        private static async Task RunAsync()
        {
            string s = await StartAsync("A");
            Console.WriteLine(s);
        }

        private static async Task<string> StartAsync(string data)
        {
            // TODO: Add to dictionary
            var id = Guid.NewGuid();
            var tcs = new TaskCompletionSource<string>();
            _map.TryAdd(id, tcs);
            _queue.Add(new KeyValuePair<Guid, string>(id, data));
            // await
            var result = await tcs.Task;
            return result;
        }
    }
}
