using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ThreadAffinity
{
    class Program
    {
        private static readonly int ITERATIONS = 3; // Environment.ProcessorCount * 3;

        private static Action _execute;
        static void Main(string[] args)
        {
            //ControlAffinity.SetProcAffinity(0,2,6);
            _execute = ControlAffinity.Run;
            //_execute = ControlAffinity.RunWithNetAffinity; // Not doing the affinity

            //_execute = ControlAffinity.RunWithWin32AffinityMask;

            #region Console.WriteLine

            Console.WriteLine("Thread");
            Console.WriteLine();

            #endregion // Console.WriteLine

            ExecThread();

            #region Console.WriteLine

            Console.WriteLine();
            Console.WriteLine(new string('-', 80));
            Console.WriteLine();
            Console.WriteLine("Task");
            Console.WriteLine();

            #endregion // Console.WriteLine

            ExecTask();

            #region Console.WriteLine

            Console.WriteLine();
            Console.WriteLine(new string('-', 80));
            Console.WriteLine();
            Console.WriteLine("Parallel.For");
            Console.WriteLine();
            GCcollect();

            #endregion // Console.WriteLine

            //Parallel.For(0, iterations, i => ControlAffinity.Run());
            Parallel.For(0, ITERATIONS, i => _execute());
            
            Console.ReadKey();
        }

        #region ExecThread

        private static void ExecThread()
        {
            Thread[] trd = new Thread[ITERATIONS];
            for (int i = 0; i < ITERATIONS; i++)
            {
                trd[i] = new Thread(() => _execute());
                trd[i].Start();
            }
            for (int i = 0; i < ITERATIONS; i++)
            {
                trd[i].Join();
            }
        }

        #endregion // ExecThread

        #region ExecTask

        private static void ExecTask()
        {
            Task[] tsk = new Task[ITERATIONS];
            for (int i = 0; i < ITERATIONS; i++)
            {
                tsk[i] = Task.Factory.StartNew(_execute);
            }

            Task.WaitAll(tsk);
        }

        #endregion // ExecTask

        #region GCcollect

        private static void GCcollect()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        #endregion // GCcollect
    }
}
