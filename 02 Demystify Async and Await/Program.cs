using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace Bnaya.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            Info("Main");
            Task t = CanBeTrickyAsync();
            //Task t = AlsoTrickyAsync();
            //Task t = ExpectedAsync();
            //Task t = MultiAwaitAsync();
            Info("Main");
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


        //private static Task CanBeTrickyAsync()
        //{
        //    Info("CanBeTrickyAsync Start");
        //    Thread.Sleep(2000);
        //    Info("CanBeTrickyAsync End");
        //    return Task.CompletedTask;
        //}

        #endregion // CanBeTrickyAsync

        #region AlsoTrickyAsync

        private static async Task AlsoTrickyAsync()
        {
            Info("AlsoTrickyAsync Start");
            await Task.CompletedTask;
            Thread.Sleep(2000);
            Info("AlsoTrickyAsync End");
        }

        #endregion // AlsoTrickyAsync

        #region CanBeTrickyDemystifyAsync

        private static Task CanBeTrickyDemystifyAsync()
        {
            Info("CanBeTrickyAsync Start");
            Thread.Sleep(2000);
            Info("CanBeTrickyAsync End");
            return Task.CompletedTask;
        }

        #endregion // CanBeTrickyDemystifyAsync

        #region ExpectedAsync

        private static async Task ExpectedAsync()
        {
            Info("ExpectedAsync Start");
            await Task.Delay(2000);
            Info("ExpectedAsync End");
        }

        #endregion // ExpectedAsync

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
            Console.WriteLine($"{title} [{trd.ManagedThreadId}]");
        }

        #endregion // Info
    }
}