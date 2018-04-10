using System;
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
    public class AsyncImmutableContext: IAsyncContext // Best practice: handle immutable content 
    {
        public readonly static IAsyncContext Instance = new AsyncImmutableContext();

        private AsyncLocal<IImmutableList<string>> _contexts =
            new AsyncLocal<IImmutableList<string>>();

        private IImmutableList<string> _context =>
            _contexts.Value ?? ImmutableList<string>.Empty;

        #region Add

        public IEnumerable<string> Add<T>(T value, [CallerMemberName]string source =null)
        {
            var current = _context.Add($"{source}: {value}");
            _contexts.Value = current;
            return current;
        }

        #endregion // Add

        #region GetContext

        public string GetContext<T>(
            T title,
            [CallerMemberName]string source = null)
        {
            string items = string.Join("\r\n", _context);
            return $@"============= {source} {title} ===========
{items}";
        }

        #endregion // GetContext
    }
}
