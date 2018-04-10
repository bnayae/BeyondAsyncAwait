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
    public class AsyncContext: IAsyncContext // Consider using immutable content 
    {
        public readonly static IAsyncContext Instance = new AsyncContext();

        private AsyncLocal<ConcurrentQueue<string>> _contexts =
            new AsyncLocal<ConcurrentQueue<string>>();

        #region Add

        public IEnumerable<string> Add<T>(T value, [CallerMemberName]string source =null)
        {
            ConcurrentQueue<string> context = _contexts.Value;
            lock (_contexts)
            {
                if (_contexts.Value == null) // double check within the lock
                    _contexts.Value = new ConcurrentQueue<string>();
                context = _contexts.Value;
            }
            context.Enqueue($"{source}: {value}");
            return context;
        }

        #endregion // Add

        #region GetContext

        public string GetContext<T>(
            T title,
            [CallerMemberName]string source = null)
        {
            var context = _contexts.Value ?? Enumerable.Empty<string>();
            string items = string.Join("\r\n", context);
            return $@"============= {source} {title} ===========
{items}";
        }

        #endregion // GetContext
    }
}
