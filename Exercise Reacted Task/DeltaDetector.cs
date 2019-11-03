using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Exercise_Reacted_Task
{
    public class DeltaDetector
    {
        public DeltaDetector(IProducer producer, int notifyOnDelta)
        {
            // TODO: keep producer.Value in memory
        }

        public Task<int> WhenCrossDelta
        {
            get
            {
                throw new NotImplementedException();
                // TODO: return Task which sample the producer.Value (on the constructor) 
                //          and complete when the current value 
                //          cross the delta (from the sample)
                //          with task result = notifyOnDelta
            }
        }
    }
}
