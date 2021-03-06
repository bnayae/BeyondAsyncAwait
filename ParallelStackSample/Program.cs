﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sela.Samples
{
    static class Program
    {
        private static int _count = 0;
        private const int WORK_DURATION_SECONDS = 2;

        static void Main(string[] args)
        {
            //var t = new Thread(() =>
            //{
            //    while (true)
            //    {
            //        A(10);
            //    }
            //});
            //t.Start();
            Parallel.For(0, 30, A);
        }

        private static void Q(int i)
        {
            if(i == 555)
                Console.ReadKey();
            if (i == 7777)
                E();
            //Q(555);
        }

        private static void A(int i)
        {
            var s = new string('*', (i + 1));
            Console.Write(nameof(A));
            //if(i < 2)
            //    Task.Factory.StartNew(() => Q(999));
            B(s);

        }

        private static void B(string i)
        {
            Console.Write(nameof(B));
            DoWork();
            int count = Interlocked.Increment(ref _count);
            if (count % 3 == 0)
                C(count);
            else //if (count % 3 == 1)
                D(count);
            //else
            //    Task.Factory.StartNew(() => Q(7777), TaskCreationOptions.AttachedToParent);

        }

        private static void C(int count)
        {
            Console.Write(nameof(C));
            DoWork();
            D(count);
        }

        private static void D(int count)
        {
            Console.Write(nameof(D));
            if (count % 2 == 0)
                E();
            else
                F();
        }

        private static void E()
        {
            Console.Write(nameof(E));
            DoWork();
            Console.ReadKey();
        }

        private static void F()
        {
            Console.Write(nameof(F));
            DoWork();
            Console.ReadKey();
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void DoWork()
        {
            int durationMilliseconds = WORK_DURATION_SECONDS * 1000;
            if (Environment.TickCount % 3 == 0)
                durationMilliseconds /= 10;
            var sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < durationMilliseconds)
            {
                if(Environment.TickCount % 1000 == 0)
                    Console.Write(".");
            }
            sw.Stop();
        }
    }
}
