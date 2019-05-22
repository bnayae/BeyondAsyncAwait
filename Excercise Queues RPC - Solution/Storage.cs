using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Bnaya.Samples
{
    public class Storage
    {
        public readonly static BlockingCollection<CorrelationItem<int>> RequestChannel =
            new BlockingCollection<CorrelationItem<int>>();
        public readonly static BlockingCollection<CorrelationItem<string>> ResponseChannel =
            new BlockingCollection<CorrelationItem<string>>();
    }
}
