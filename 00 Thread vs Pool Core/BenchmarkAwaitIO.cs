using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Bnaya.Samples
{
    public class BenchmarkAwaitIO : IBenchmark
    {
        #region Constants

        private const int THREAD_LEVEL = 10_000; //50_000;
        private const int WORK_LEVEL = 100; //10_000;

        #endregion // Constants

        #region CountdownEvents

        private static readonly CountdownEvent _cd = new CountdownEvent(THREAD_LEVEL);

        #endregion // CountdownEvents

        #region ExecPool

        public void ExecPool()
        {
            _cd.Reset();
            for (int i = 0; i < THREAD_LEVEL; i++)
            {
                ThreadPool.QueueUserWorkItem((WaitCallback)(async _ =>
                        {
                            await UnitOfWork();
                            BenchmarkAwaitIO._cd.Signal();
                        }), null);
            }
            _cd.Wait();
        }

        #endregion // ExecPool

        #region ExecThread

        public void ExecThread()
        {
            _cd.Reset();
            for (int i = 0; i < THREAD_LEVEL; i++)
            {
                Thread t = new Thread(async () =>
                    {
                        await UnitOfWork();
                        _cd.Signal();
                    });
                t.IsBackground = true;
                t.Name = "T " + i;
                t.Start();
            }
            _cd.Wait();
        }

        #endregion // ExecThread

        #region UnitOfWork

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static async Task UnitOfWork()
        {
            await Task.Delay(WORK_LEVEL); // Thread goes back to the pool
        }

        #endregion // UnitOfWork  
    }
}
