using System;
using System.Threading;

namespace _000_ThreadPool
{
    class Program
    {
        private static int _count = 25;

        static void Main(string[] args)
        {
            for (int i = 0; i < 25; i++)
            {
                ThreadPool.QueueUserWorkItem(state =>
                {
                    Console.Write("-");
                    Thread.Sleep(5000); // thread pool starvetion
                    Console.Write("|");
                    Interlocked.Decrement(ref _count);
                }, i);
            }
            while (_count != 0)
            {
                Console.Write(".");
                Thread.Sleep(100);
            }
            Console.ReadKey();
        }
    }
}