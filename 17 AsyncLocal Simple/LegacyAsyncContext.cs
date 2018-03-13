using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _17_AsyncLocal_Simple
{
    public class LegacyAsyncContext
    {
        private const string SLOT = "Context";

        public async Task Exec1Async(string s, int i)
        {
            // Don't confuse with CallContext.SetData
            CallContext.LogicalSetData(SLOT, i);
            await Task.Delay(i % 2 * 500);
            await Exec2Async(s);
        }

        private async Task Exec2Async(string s)
        {
            await Task.Delay(100);
            int i = (int)CallContext.LogicalGetData(SLOT);

            Console.WriteLine($"Context of {s} = {i}");
        }
    }
}
