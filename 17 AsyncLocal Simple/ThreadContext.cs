using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _17_AsyncLocal_Simple
{
    public class ThreadContext
    {
        private ThreadLocal<string> _contexts = new ThreadLocal<string>();

        public async Task Exec1Async(string s, int i)
        {
            _contexts.Value = i.ToString();
            await Task.Delay(i % 2 * 500);
            await Exec2Async(s);
        }

        private async Task Exec2Async(string s)
        {
            // thread pool
            Console.WriteLine($"Thread local context of {s} = {_contexts.Value}");
        }
    }
}
