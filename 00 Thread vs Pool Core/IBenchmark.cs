using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Bnaya.Samples
{
    public interface IBenchmark
    {
        void ExecPool();
        void ExecThread();
    }
}
