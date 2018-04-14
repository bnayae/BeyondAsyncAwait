using System;
using System.Threading;
using System.Threading.Tasks;
#pragma warning disable Await1 // Method is not configured to be awaited
#pragma warning disable CS0618 // Type or member is obsolete

namespace Bnaya.Samples
{
    static class Program
    {
        static void Main(string[] args)
        {
            Task a = CheckDeadlockAsync();
            //Task b = CheckTimeoutAsync();
            //Task c = MultiExceptionAsync();
            //Task d = ExecAsync();

            //SafeCancellation();

            Console.ReadKey();
        }

        private static async Task CheckDeadlockAsync()
        {
            try
            {
                Task t = Task.Delay(1000);
                // check for potential deadlock
                if (await t.IsTimeoutAsync(TimeSpan.FromMilliseconds(100)).ConfigureAwait(false))
                    Console.WriteLine("Potential deadlock");
                await t.ConfigureAwait(false);
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

        private static void SafeCancellation()
        {
            var cts = new CancellationTokenSource();
            cts.Token.Register(() => throw new InvalidCastException("Shit may happen"));

            #region Risky Cancellation

            cts.Cancel();

            #endregion // Risky Cancellation

            #region Safer Cancellation

            //try
            //{
            //    cts.Cancel();
            //}
            //catch (Exception)
            //{
            //    // TODO: Log
            //}

            #endregion // Safer Cancellation

            #region CancelSafe

            //cts.CancelSafe();

            #endregion // CancelSafe

            #region CancelSafe (advance)

            //if (!cts.CancelSafe(out Exception ex))
            //    Console.WriteLine($"Cancellation throw: {ex.Format()}");

            #endregion // CancelSafe (advance)
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
