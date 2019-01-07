using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bnaya.Samples
{
    class Program
    {
        private static readonly Random _rnd = new Random(Guid.NewGuid().GetHashCode());

        public static async Task Main()
        {
            var sw = new Stopwatch();
            //var a = new int[] { 3, 5, 2, 0, 1, 8, 7, 9 };
            var a = Enumerable.Range(0, 10_000).Select(_ => _rnd.Next(0, 1_000)).ToArray();

            Show("Unsort", a, sw);

            for (int i = 0; i < 5; i++)
            {
                sw.Restart();
                var b = QuickSortSimplify.Sort(a).ToArray();
                sw.Stop();
                Show("Sync ", b, sw);


                sw.Restart();
                var c = await QuickSortAsync.SortAsync(a);
                sw.Stop();
                Show("Async", c, sw);
            }
            Console.ReadKey();
        }

        private static void Show(string title, IEnumerable<int> a, Stopwatch sw) => 
            Console.WriteLine($"{title} [{sw.Elapsed.TotalMilliseconds:N4}]: {string.Join(",", a.Take(50))}...");
    }
}
