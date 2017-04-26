#if Fx
using BenchmarkDotNet.Running;
#endif
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Bnaya.Samples
{
    class Program
    {
        private readonly static IBenchmark _banchmark = new BenchmarkCompute();
        //private readonly static IBenchmark _banchmark = new BenchmarkIO(); // pool starvation
        //private readonly static IBenchmark _banchmark = new BenchmarkAwaitIO(); // pool starvation

        static void Main(string[] args)
        {
            Console.WriteLine($"Stat: {Process.GetCurrentProcess().ProcessName}");

            #region Warm-up

            Thread t = new Thread(() => { });
            t.Start();
            ThreadPool.QueueUserWorkItem(state => { }, null);
            Thread.Sleep(200);

            #endregion // Warm-up


#if Fx
            //var summary = BenchmarkRunner.Run<BenchmarkCompute>();
            ////var summary = BenchmarkRunner.Run<BenchmarkIO>();
            //Console.WriteLine(summary);
#endif

            for (int i = 0; i < 3; i++)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Measure(_banchmark.ExecPool, "ExecPool");
                Console.ForegroundColor = ConsoleColor.White;
                Measure(_banchmark.ExecThread, "ExecThread");
                Console.ResetColor();
            }

            Console.WriteLine("Done");
            Console.ReadKey(true);
        }

        #region Measure

        private static void Measure(Action operation, string title)
        {
            Thread.Sleep(100);
            var sw = Stopwatch.StartNew();

            operation();

            sw.Stop();
            Console.WriteLine($"\r\n{title}: Duration = \t{sw.Elapsed.TotalSeconds:##0.00}");
        }

        #endregion // Measure
    }
}
