using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

#pragma warning disable ParallelChecker // Detection of parallel issues in C#.
namespace Bnaya.Samples
{
    class Program
    {
        private static int _count = 25;
        private static readonly TimeSpan DURATION = TimeSpan.FromSeconds(10);

        static void Main(string[] args)
        {
            for (int i = 0; i < 25; i++)
            {
                ThreadPool.QueueUserWorkItem(state =>
                {
                    Console.Write(">");
                    ExecuteIoLike(DURATION);
                    //ExecuteComputeLike(DURATION);
                    Console.Write("|");
                    var x = Interlocked.Decrement(ref _count);
                }, i);
            }
            while (_count != 0)
            {
                Console.Write(".");
                Thread.Sleep(100);
            }
            Console.ReadKey();
        }

        private static void ExecuteIoLike(TimeSpan duration)
        {
            Thread.Sleep(duration); // thread pool starvation
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void ExecuteComputeLike(TimeSpan duration)
        {
            var sw = Stopwatch.StartNew();
            while (sw.Elapsed < duration)
            {
               
            }
        }
    }
}