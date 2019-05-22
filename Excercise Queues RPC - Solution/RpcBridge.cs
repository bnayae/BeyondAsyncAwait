using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Bnaya.Samples.Storage;

namespace Bnaya.Samples
{
    public class RpcBridge
    {
        private readonly ConcurrentDictionary<Guid, TaskCompletionSource<string>> _map =
            new ConcurrentDictionary<Guid, TaskCompletionSource<string>>();

        public RpcBridge()
        {
            Thread t = new Thread(WatchLoop)
            {
                Name = nameof(WatchLoop)
            };
            t.Start();
        }

        private void WatchLoop()
        {
            foreach (var response in ResponseChannel.GetConsumingEnumerable())
            {
                if (_map.TryRemove(response.Correlation,
                                    out TaskCompletionSource<string> tcs))
                {
                    tcs.TrySetResult(response.Value);
                }
            }
        }

        public Task<string> SendAsync(int request)
        {
            var tcs = new TaskCompletionSource<string>();
            var item = new CorrelationItem<int>(request);
            _map.TryAdd(item.Correlation, tcs);
            RequestChannel.TryAdd(item);
            return tcs.Task;
        }
    }
}
