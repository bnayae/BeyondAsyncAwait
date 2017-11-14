using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Bnaya.Samples
{
    public class BenchmarkCompute : IBenchmark
    {
        #region Constants

        private const int THREAD_LEVEL = 10000;
        private const int WORK_LEVEL = 10000;

        #endregion // Constants

        #region CountdownEvents

        private static readonly CountdownEvent _cdPool = new CountdownEvent(THREAD_LEVEL);
        private static readonly CountdownEvent _cdThread = new CountdownEvent(THREAD_LEVEL);

        #endregion // CountdownEvents

        #region ExecPool

        public void ExecPool()
        {
            _cdPool.Reset();
            for (int i = 0; i < THREAD_LEVEL; i++)
            {
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    UnitOfWork();
                    _cdPool.Signal();
                }, null);
            }
            _cdPool.Wait();
        }

        #endregion // ExecPool

        #region ExecThread

        public void ExecThread()
        {
            _cdThread.Reset();
            for (int i = 0; i < THREAD_LEVEL; i++)
            {
                Thread t = new Thread(() =>
                {
                    UnitOfWork();
                    _cdThread.Signal();
                });
                t.IsBackground = true;
                t.Name = "T " + i;
                t.Start();
            }
            _cdThread.Wait();
        }

        #endregion // ExecThread

        #region UnitOfWork

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void UnitOfWork()
        {
            for (int i = 0; i < WORK_LEVEL; i++)
            {
            }
        }

        #endregion // UnitOfWork   
    }
}
