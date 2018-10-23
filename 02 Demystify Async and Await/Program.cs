using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
#pragma warning disable RCS1163 // Unused parameter.
#pragma warning disable CC0068 // Unused Method
#pragma warning disable CC0022 // Should dispose object

namespace Bnaya.Samples
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Info(nameof(Main));
            //Task t = CanBeTrickyAsync();
            //Task t = CanBeTrickyDemystifyAsync();
            //Task t = AlsoTrickyAsync();
            Task t = ExpectedAsync();
            //Task t = ExpectedDemystifyAsync();
            //Task t = MultiAwaitAsync();
            Info(nameof(Main));
            while (!t.IsCompleted)
            {
                Console.Write(".");
                Thread.Sleep(50);
            }

            Console.ReadKey();
        }

        #region CanBeTrickyAsync

        private static async Task CanBeTrickyAsync()
        {
            Info("CanBeTrickyAsync Start");
            Thread.Sleep(2000);
            Info("CanBeTrickyAsync End");
        }

        #endregion // CanBeTrickyAsync

        #region CanBeTrickyDemystifyAsync

        private static Task CanBeTrickyDemystifyAsync()
        {
            Info("CanBeTrickyAsync Start");
            Thread.Sleep(2000);
            Info("CanBeTrickyAsync End");
            return Task.CompletedTask;
        }

        #endregion // CanBeTrickyDemystifyAsync

        #region AlsoTrickyAsync

        private static async Task AlsoTrickyAsync()
        {
            Info("AlsoTrickyAsync Start");
            await Task.CompletedTask;
            Thread.Sleep(2000);
            Info("AlsoTrickyAsync End");
        }

        #endregion // AlsoTrickyAsync

        #region AlsoTrickyDemystifyAsync

        private static Task AlsoTrickyDemystifyAsync()
        {
            Info("AlsoTrickyDemystifyAsync Start");
            Task t = Task.CompletedTask;
            if (t.IsCompleted)
            {
                Info("AlsoTrickyDemystifyAsync End (Optimized)");
                return Task.CompletedTask;
            }
            Task r = t.ContinueWith(c =>
            {
                //await MyDelay(TimeSpan.FromSeconds(2));
                Info("AlsoTrickyDemystifyAsync End");
            });
            return r;
        }

        #endregion // AlsoTrickyDemystifyAsync

        #region ExpectedAsync

        private static async Task ExpectedAsync()
        {
            Info("ExpectedAsync Start");
            await Task.Delay(2000);
            //await MyDelay(TimeSpan.FromSeconds(2));
            Info("ExpectedAsync End");
        }

        #endregion // ExpectedAsync

        #region ExpectedDemystifyAsync

        private static Task ExpectedDemystifyAsync()
        {
            Info("ExpectedAsync Start");
            Task t = Task.Delay(2000); // Task.CompletedTask;
            if (t.IsCompleted)
            {
                Info("ExpectedAsync End (Optimized)");
                return Task.CompletedTask;
            }
            Task r = t.ContinueWith(c =>
            {
                //await MyDelay(TimeSpan.FromSeconds(2));
                Info("ExpectedAsync End");
            });
            return r;
        }

        #endregion // ExpectedDemystifyAsync

        #region RunSomethingAsync

        //private static async Task RunSomethingAsync()
        //{
        //    Info("ExpectedAsync Start");
        //    Task.Run(() =>
        //    {
        //    });
        //    Info("ExpectedAsync End");
        //}

        #endregion // RunSomethingAsync

        #region MultiAwaitAsync

        private static async Task MultiAwaitAsync()
        {
            var sw = Stopwatch.StartNew();
            Info("ExpectedAsync Start");
            await Task.Delay(2000);
            Info("ExpectedAsync Processing");
            await Task.Delay(2000);
            Info("ExpectedAsync End");
            sw.Stop();
            Console.WriteLine($"Duration = {sw.Elapsed:hh':'mm':'ss'.'fff}");
        }

        #endregion // MultiAwaitAsync

        #region Info

        private static void Info(string title)
        {
            var trd = Thread.CurrentThread;
            Console.WriteLine($"{title} [{trd.ManagedThreadId}]: Pool = {trd.IsThreadPoolThread}");
        }

        #endregion // Info

        #region MyDelay

        private static Task MyDelayAsync(TimeSpan duration)
        {
            var tcs = new TaskCompletionSource<object>();
            // GC may collect the timer!!!
            var tmr = new Timer(s => tcs.TrySetResult(null), null, duration, TimeSpan.Zero);
            return tcs.Task;
        }

        #endregion // MyDelay
    }
}