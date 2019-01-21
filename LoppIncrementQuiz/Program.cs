using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LoppIncrementQuiz
{
    class Program
    {
        private static int _count = 0;
        private const int LIMIT = 100_000_000;


        static void Main(string[] args)
        {
            for (int i = 0; i < 5; i++)
            {
                _count = 0;
                Sync();
                //_count = 0;
                //Unsafe();
                //_count = 0;
                //WithInterlocked();
                //_count = 0;
                //WithLock();
                //_count = 0;
                //DataLocality();
                Console.WriteLine("-------------------");
            }

            Console.ReadKey();
        }


        public static void Sync()
        {
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < LIMIT; i++)
            {
                _count++;
            }
            sw.Stop();
            Console.WriteLine($"Sync: {sw.ElapsedMilliseconds}, Count = {_count}");
        }


        public static void Unsafe()
        {
            var sw = Stopwatch.StartNew();
            using (var cd = new CountdownEvent(LIMIT))
            {
                for (int i = 0; i < LIMIT; i++)
                {
                    ThreadPool.QueueUserWorkItem(s =>
                    {
                        _count++;
                        cd.Signal();
                    }, null);
                }
                cd.Wait();
                sw.Stop();
            }
            Console.WriteLine($"Unsafe: {sw.ElapsedMilliseconds}, Count = {_count}");
        }

        public static void WithInterlocked()
        {
            var sw = Stopwatch.StartNew();
            using (var cd = new CountdownEvent(LIMIT))
            {
                for (int i = 0; i < LIMIT; i++)
                {
                    ThreadPool.QueueUserWorkItem(s =>
                    {
                        Interlocked.Increment(ref _count);
                        cd.Signal();
                    }, null);
                }
                cd.Wait();
                sw.Stop();
            }
            Console.WriteLine($"Interlocked: {sw.ElapsedMilliseconds}, Count = {_count}");
        }


        public static void WithLock()
        {
            object _gate = new object();
            var sw = Stopwatch.StartNew();
            using (var cd = new CountdownEvent(LIMIT))
            {
                for (int i = 0; i < LIMIT; i++)
                {
                    ThreadPool.QueueUserWorkItem(s =>
                    {
                        lock (_gate)
                        {
                            _count++;
                        }
                        cd.Signal();
                    }, null);
                }
                cd.Wait();
                sw.Stop();
            }
            Console.WriteLine($"Lock: {sw.ElapsedMilliseconds}, Count = {_count}");
        }

        public static void DataLocality()
        {
            var sw = Stopwatch.StartNew();
            int limit = Environment.ProcessorCount;
            using (var cd = new CountdownEvent(limit))
            {
                for (int i = 0; i < limit; i++)
                {
                    ThreadPool.QueueUserWorkItem(s =>
                    {
                        int j = 0;
                        for (int k = 0; k < LIMIT / limit /* last + % */; k++)
                        {
                            j++;
                        }
                        Interlocked.Add(ref _count, j);
                        cd.Signal();
                    }, null);
                }
                cd.Wait();
                sw.Stop();
            }
            Console.WriteLine($"Data Locality: {sw.ElapsedMilliseconds}, Count = {_count}");
        }

    }
}
