#if Fx
using BenchmarkDotNet.Attributes;
#endif
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Bnaya.Samples
{
    public class BenchmarkAwaitIO : IBenchmark
    {
        private const int THREAD_LEVEL = 10000; //50000;
        private const int WORK_LEVEL = 1000; //10000;
        private static readonly CountdownEvent _cdPool = new CountdownEvent(THREAD_LEVEL);
        private static readonly CountdownEvent _cdThread = new CountdownEvent(THREAD_LEVEL);

#if Fx
        [Benchmark]
#endif
        public void ExecPool()
        {
            _cdPool.Reset();
            for (int i = 0; i < THREAD_LEVEL; i++)
            {
                ThreadPool.QueueUserWorkItem(async _ =>
                        {
                            await UnitOfWork();
                            _cdPool.Signal();
                        }, null);
            }
            _cdPool.Wait();
        }


#if Fx
        [Benchmark]
#endif
        public void ExecThread()
        {
            _cdThread.Reset();
            for (int i = 0; i < THREAD_LEVEL; i++)
            {
                Thread t = new Thread(async () =>
                    {
                        await UnitOfWork();
                        _cdThread.Signal();
                    });
                t.IsBackground = true;
                t.Name = "T " + i;
                t.Start();
            }
            _cdThread.Wait();
        }

        #region UnitOfWork

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private async Task UnitOfWork()
        {
            await Task.Delay(WORK_LEVEL); // Thread goes back to the pool
        }

        #endregion // UnitOfWork  
    }
}
