using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bnaya.Samples
{
    public class MyLock
    {
        private readonly ConcurrentQueue<TaskCompletionSource<object>> _lockes = 
            new ConcurrentQueue<TaskCompletionSource<object>>();

        public async Task<IDisposable> LockAsync()
        {
            var tcs = new TaskCompletionSource<object>();
            _lockes.Enqueue(tcs);
            while (true)
            {
                if (!_lockes.TryPeek(out var current))
                    throw new IndexOutOfRangeException("Queue should never be empty");
                if (tcs == current)
                    break;
                await current.Task;
            }
            var lck = new ActualLock(() =>
            {
                if(!_lockes.TryDequeue(out _))
                    throw new IndexOutOfRangeException("Queue should never be empty");
                tcs.TrySetResult(null);
            });
            return lck;
        }

        private class ActualLock : IDisposable
        {
            private readonly Action _release;

            public ActualLock(Action release)
            {
                _release = release;
            }

            public void Dispose() => _release();
        }
    }
}
