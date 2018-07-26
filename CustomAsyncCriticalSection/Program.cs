using System;
using System.Linq;
using System.Threading.Tasks;

namespace Bnaya.Samples
{
    class Program
    {
        private static MyLock _lock = new MyLock();

        static async Task Main(string[] args)
        {
            Console.WriteLine("Start");

            var tasks = Enumerable.Range(0, 40)
                            .Select(ExecAsync);
            await Task.WhenAll(tasks);

            Console.WriteLine("Done");
            Console.ReadKey();
        }

        private static async Task ExecAsync(int i)
        {
            using (await _lock.LockAsync().ConfigureAwait(false))
            {
                Console.Write($"-> {i}");
                await Task.Delay(500).ConfigureAwait(false);
                Console.Write("| ");
            }
        }
    }
}
