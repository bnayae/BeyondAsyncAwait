using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Exercise_Reacted_Task
{
    public class Producer : IProducer
    {
        private double _value;
        private Timer _tmr;
        private Random _rnd = new Random();
        public event Action<double> Changed;

        public Producer()
        {
            _tmr = new Timer(OnTime, null, 20, 20);
        }

        public double Value => _value;

        private void OnTime(object state)
        {
            var delta = _rnd.NextDouble() - 0.5;
            // TODO: add delta to _value (safe)
            // TODO: notify Changed
        }

    }
}
