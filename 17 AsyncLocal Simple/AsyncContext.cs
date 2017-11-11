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
        private AsyncLocal<string> _contexts = new AsyncLocal<string>();

        public async Task Exec1Async(string s, int i)
        {
            _contexts.Value = i.ToString();
            await Task.Delay(i % 2 * 500);
            await Exec2Async(s);
        }

        private async Task Exec2Async(string s)
        {
            Console.WriteLine($"Context of {s} = {_contexts.Value}");
        }
    }
}
