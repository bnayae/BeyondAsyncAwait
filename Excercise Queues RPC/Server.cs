using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bnaya.Samples
{
    public class Server
    {
        private readonly BlockingCollection<CorrelationItem<int>> _requestChannel;
        private readonly BlockingCollection<CorrelationItem<string>> _responseChannel;

        public Server(
            BlockingCollection<CorrelationItem<int>> requestChannel,
            BlockingCollection<CorrelationItem<string>> responseChannel)
        {
            _requestChannel = requestChannel;
            _responseChannel = responseChannel;
            Thread t = new Thread(WatchLoop)
            {
                Name = nameof(WatchLoop)
            };
            t.Start();
        }

        private void WatchLoop()
        {
            foreach (var r in _requestChannel.GetConsumingEnumerable())
            {
                var request = r; // capture variable
                Task.Run(() =>
                {
                    Thread.Sleep(request.Value * 1000); // bad practice
                    var item = new CorrelationItem<string>(request.Correlation, $"Data of {request.Value}");
                    _responseChannel.TryAdd(item);
                });
            }
        }
    }
}
