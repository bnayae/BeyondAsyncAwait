using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

// see also: https://blog.stephencleary.com/2013/04/implicit-async-context-asynclocal.html

namespace Bnaya.Samples
{
    public interface IAsyncContext
    {
        string GetContext<T>(T title, [CallerMemberName]string source = null);

        IEnumerable<string> Add<T>(T value, [CallerMemberName]string source = null);
    }
}
