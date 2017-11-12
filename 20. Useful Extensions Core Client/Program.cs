using System;
using System.Threading;
using System.Threading.Tasks;

namespace _20._Useful_Extensions_Core_Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Task _ = CheckDeadlockAsync();
            _ = CheckTimeoutAsync();
            _ = MultiExceptionAsync();
            _ = ExecAsync();

            var cts = new CancellationTokenSource();
            cts.Token.RegisterWeak(() => throw new InvalidCastException("Shit may happen"));
            if(!cts.CancelSafe())
                Console.WriteLine("shit does happen");

            Console.ReadKey();
        }

        private static async Task CheckDeadlockAsync()
        {
            try
            {
                Task t = Task.Delay(1000);
                // check for potential deadlock
                if (await t.IsTimeoutAsync(TimeSpan.FromMilliseconds(100)))
                    Console.WriteLine("Potential deadlock");
                await t;
                Console.WriteLine("Eventually it pass");
            }
            catch (TimeoutException)
            {
                Console.WriteLine("Took longer than expected");
            }
        }

        private static async Task CheckTimeoutAsync()
        {
            try
            {
                await Task.Delay(1000).WithTimeout(TimeSpan.FromMilliseconds(100));
                throw new Exception("Expecting timeout");
            }
            catch (TimeoutException)
            {
                Console.WriteLine("Took longer than expected");
            }
        }

        private static async Task MultiExceptionAsync()
        {
            Task a = Task.Run(() => throw new IndexOutOfRangeException("Error A"));
            Task b = Task.Run(() => throw new IndexOutOfRangeException("Error B"));

            try
            {
                await Task.WhenAll(a, b).ThrowAll();
            }
            catch (AggregateException exs)
            {
                var flatten = exs.Flatten();
                Console.WriteLine($"Catch {flatten.InnerExceptions.Count} exceptions");
                foreach (var ex in flatten.InnerExceptions)
                {
                    Console.WriteLine(ex.GetBaseException().Message);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Catch single exception");
            }
        }

        private static async Task ExecAsync()
        {
            try
            {
                await Exec1Async();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Format());
            }

        }
        private static async Task Exec1Async()
        {
            await Task.Delay(1);
            await Exec2Async();
        }
        private static async Task Exec2Async()
        {
            await Task.Delay(1);
            await Exec3Async();
        }
        private static async Task Exec3Async()
        {
            await Task.Delay(1);
            throw new InvalidOperationException("Some error");
        }
    }
}
