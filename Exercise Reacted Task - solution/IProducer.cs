using System;

namespace Exercise_Reacted_Task
{
    public interface IProducer
    {
        double Value { get; }

        event Action<double> Changed;
    }
}