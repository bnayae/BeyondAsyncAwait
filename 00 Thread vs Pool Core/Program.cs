using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Bnaya.Samples
{
    class Program
    {
        private readonly static IBenchmark _banchmarkCompute = new BenchmarkCompute();
        private readonly static IBenchmark _banchmarkIO = new BenchmarkIO(); // pool starvation
        private readonly static IBenchmark _banchmarkAwaitIO = new BenchmarkAwaitIO(); // pool starvation

        static void Main(string[] args)
        {
            Console.WriteLine($"Stat: {Process.GetCurrentProcess().ProcessName}");

            #region _banchmark = switch (Console.ReadKey())

            Console.WriteLine(@"Select
1. Compute
2. IO blocking
3. IO with Await");
            char c = Console.ReadKey(true).KeyChar;
            IBenchmark _banchmark;
            switch (c)
            {
                case '1':
                    _banchmark = _banchmarkCompute;
                    break;
                case '2':
                    _banchmark = _banchmarkIO;
                    break;
                case '3':
                    _banchmark = _banchmarkAwaitIO;
                    break;
                default:
                    throw new NotSupportedException();
            }

            #endregion // _banchmark =  = switch (Console.ReadKey())

            #region Warm-up

            Thread t = new Thread(() => { });
            t.Start();
            ThreadPool.QueueUserWorkItem(state => { }, null);
            Thread.Sleep(200);

            #endregion // Warm-up

            for (int i = 0; i < 6; i++)
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
