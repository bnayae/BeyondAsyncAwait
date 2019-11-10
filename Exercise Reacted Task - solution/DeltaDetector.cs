using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Exercise_Reacted_Task
{
    public class DeltaDetector
    {
        private readonly double _initValue;
        private readonly IProducer _producer;
        private readonly int _notifyOnDelta;
        private readonly TaskCompletionSource<int> _tsc = new TaskCompletionSource<int>();

        public DeltaDetector(IProducer producer, int notifyOnDelta)
        {
            _initValue = producer.Value;
            _producer = producer;
            producer.Changed += OnChanged;
            _notifyOnDelta = notifyOnDelta;
            // TODO: keep producer.Value in memory
        }

        private void OnChanged(double item)
        {
            double gap = Math.Abs(_initValue - item) ;
            Trace.Write($"{gap}, ");
            if (_notifyOnDelta < gap)
            {
                _producer.Changed -= OnChanged;
                _tsc.TrySetResult(_notifyOnDelta);
            }
        }

        public Task<int> WhenCrossDelta
        {
            get
            {
                return _tsc.Task;
                //throw new NotImplementedException();
                // TODO: return Task which sample the producer.Value (on the constructor) 
                //          and complete when the current value 
                //          cross the delta (from the sample)
                //          with task result = notifyOnDelta
            }
        }
    }
}
