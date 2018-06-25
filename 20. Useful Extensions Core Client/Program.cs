using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static System.Math;
#pragma warning disable Await1 // Method is not configured to be awaited
#pragma warning disable CS0618 // Type or member is obsolete

namespace Bnaya.Samples
{
    static class Program
    {
        static void Main(string[] args)
        {
            //Task a = CheckDeadlockAsync();
            //Task b = CheckTimeoutAsync();
            //Task c = MultiExceptionAsync();
            //Task d = ExecAsync();

            //SafeCancellation();

            Task e = CompleteWhenN();

            Console.ReadKey();
        }

        #region CheckDeadlockAsync

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

        #endregion // CheckDeadlockAsync

        #region CheckTimeoutAsync

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

        #endregion // CheckTimeoutAsync

        #region SafeCancellation

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

        #endregion // SafeCancellation

        #region MultiExceptionAsync

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

        #endregion // MultiExceptionAsync

        #region CompleteWhenN

        private static async Task CompleteWhenN()
        {
            var cts = new CancellationTokenSource();
            var tasks = from i in Enumerable.Range(0, 20)
                        select SingleStepWithResultAsync(i, cts.Token);
            await tasks.When(i => i > 4 && i < 8); // more common to use it with Task<bool>
            //await tasks.WhenN(2);
            //await tasks.WhenN(2, i => i % 2 == 0);
            //await tasks.WhenN(4, cts); // cancel signaling (to none completed tasks)
            Console.WriteLine("COMPLETE");
        }

        #endregion // CompleteWhenN

        #region SingleStepAsync / SingleStepWithResultAsync

        private static async Task<int> SingleStepWithResultAsync(
                                int i,
                                CancellationToken cancellationToken)
        {
            #region ThrowIfCancellationRequested

            //cancellationToken.ThrowIfCancellationRequested();
            if (cancellationToken.IsCancellationRequested)
            {
                Console.Write("X");
                throw new OperationCanceledException();
            }

            #endregion // ThrowIfCancellationRequested
            int delay = Abs(3000 - (i * 100));

            await Task.Delay(delay);
            Console.Write($"{i}, ");
            return i;
        }

        private static async Task SingleStepAsync(int i)
        {
            int delay = Abs(3000 - (i * 100));

            await Task.Delay(delay);
            Console.Write($"{i}, ");
        }

        #endregion // SingleStepAsync / SingleStepWithResultAsync

        #region Execution Flow: ExecAsync -> Exec1Async -> Exec2Async -> Exec3Async -> throw

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

        #endregion // Execution Flow: ExecAsync -> Exec1Async -> Exec2Async -> Exec3Async -> throw
    }
}
