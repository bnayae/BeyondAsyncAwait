using System;
using System.Collections.Generic;
using System.Text;

namespace Bnaya.Samples
{
    public class CorrelationItem<T> : ICorrelated
    {
        public CorrelationItem(T value)
            : this(Guid.NewGuid(), value)
        {
        }
        public CorrelationItem(Guid correlation, T value)
        {
            Correlation = correlation;
            Value = value;
        }
        public Guid Correlation { get; }
        public T Value { get; }

        public override string ToString() => Value?.ToString();
    }
}
