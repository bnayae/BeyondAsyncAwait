using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sela.Samples
{
    class Program
    {
        private static int _count = 0;
        private const int WORK_DURATION_SECONDS = 2;

        static void Main(string[] args)
        {
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
            Console.Write("A");
            //if(i < 2)
            //    Task.Factory.StartNew(() => Q(999));
            B(s);

        }

        private static void B(string i)
        {
            Console.Write("B");
            DoWork();
            int count = Interlocked.Increment(ref _count);
            if (count % 3 == 0)
                C(count);
            //else if (count % 3 == 1)
                D(count);
            //else
            //    Task.Factory.StartNew(() => Q(7777), TaskCreationOptions.AttachedToParent);

        }

        private static void C(int count)
        {
            Console.Write("C");
            DoWork();
            D(count);
        }

        private static void D(int count)
        {
            Console.Write("D");
            if (count % 2 == 0)
                E();
            else
                F();
        }

        private static void E()
        {
            Console.Write("E");
            DoWork();
            Console.ReadKey();
        }

        private static void F()
        {
            Console.Write("F");
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
