using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bnaya.Samples
{
    public class RpcBridge<TRequest, TResponse>
    {
        private readonly BlockingCollection<CorrelationItem<TRequest>> _requestChannel;
        private readonly BlockingCollection<CorrelationItem<TResponse>> _responseChannel;
        private readonly ConcurrentDictionary<Guid, TaskCompletionSource<TResponse>> _map =
            new ConcurrentDictionary<Guid, TaskCompletionSource<TResponse>>();

        public RpcBridge(
            BlockingCollection<CorrelationItem<TRequest>> requestChannel,
            BlockingCollection<CorrelationItem<TResponse>> responseChannel)
        {
            _requestChannel = requestChannel;
            _responseChannel = responseChannel;
            Thread t = new Thread(WatchLoop);
            t.Name = nameof(WatchLoop);
            t.Start();
        }

        private void WatchLoop()
        {
            foreach (var response in _responseChannel.GetConsumingEnumerable())
            {
                if (_map.TryRemove(response.Correlation,
                                    out TaskCompletionSource<TResponse> tcs))
                {
                    tcs.TrySetResult(response.Value);
                }
            }
        }

        public Task<TResponse> SendAsync(TRequest request)
        {
            var tcs = new TaskCompletionSource<TResponse>();
            var item = new CorrelationItem<TRequest>(request);
            _map.TryAdd(item.Correlation, tcs);
            _requestChannel.TryAdd(item);
            return tcs.Task;
        }
    }
}
