using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _17_AsyncLocal_Simple
{
    public class AsyncContext
    {
        private static AsyncLocal<string> _contexts = new AsyncLocal<string>();

        public async Task Exec1Async(string s, int i)
        {
            _contexts.Value = i.ToString();
            await Task.Delay(i % 2 * 500).ConfigureAwait(false);
            await Exec2Async(s).ConfigureAwait(false);
        }

        private async Task Exec2Async(string s)
        {
            await Task.Delay(100).ConfigureAwait(false);
            Console.WriteLine($"Context of {s} = {_contexts.Value}");
        }
    }
}
