using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConcurrentDictionaryAlg
{
    class Program
    {
        private static ConcurrentDictionary<int, int> _b = new ConcurrentDictionary<int, int>();
        static void Main(string[] args)
        {
            int count = 0;
            Parallel.For(0, 100_000, i =>
            {
                _b.AddOrUpdate(i % 10, k => 1, (k, v) =>
                {
                    Interlocked.Increment(ref count); // Don't expect it to match
                    return v + 1;
                });
            });
            foreach (var item in _b)
            {
                Console.WriteLine($"[{item.Key}] = {item.Value}");
            }
            Console.WriteLine($"Done {count:N0}");
            Console.ReadKey();
        }
    }
}
