using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bnaya.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start");
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));

            //Default(cts.Token);
            Lazy(cts.Token);

            Console.ReadKey();
        }

        private static void Default(CancellationToken token)
        {
            Task a = Task.Run(() =>
            {
                Thread.Sleep(3000);
                Console.WriteLine("A");
            });

            Task b = a.ContinueWith(t => Console.WriteLine("B"), token);
            //Task b = a.ContinueWith(t => Console.WriteLine($"B is {t.Status}"), token);


            Task c = b.ContinueWith(t => Console.WriteLine("C"));
            //Task c = b.ContinueWith(t => Console.WriteLine($"C is {t.Status}"));

            //while (!b.IsCompleted)
            //{
            //    Console.Write(".");
            //    Thread.Sleep(30);
            //}
            //Console.WriteLine(b.Status);
        }

        private static void Lazy(CancellationToken token)
        {
            Task a = Task.Run(() =>
            {
                Thread.Sleep(3000);
                Console.WriteLine("A");
            });

            Task b = a.ContinueWith(t => Console.WriteLine("B"), token,
                 TaskContinuationOptions.LazyCancellation,
                 TaskScheduler.Default);

            Task c = b.ContinueWith(t => Console.WriteLine($"C is {t.Status}"));

            //while (!b.IsCompleted)
            //{
            //    Console.Write(".");
            //    Thread.Sleep(30);
            //}
            //Console.WriteLine(b.Status);
        }
    }
}