using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Bnaya.Samples
{
    public class FooBetter
    {
        private readonly static TimeSpan TIMEOUT = TimeSpan.FromMinutes(5);
        private readonly AsyncLock _gate = new AsyncLock(TimeSpan.FromSeconds(10));

        public async Task ExecAsync(int i)
        {
            using (await _gate.AcquireAsync().ConfigureAwait(false))
            {
                Console.Write($"#{i}");
                await Task.Delay(1500).ConfigureAwait(false);
                Console.Write($"{i}# ");
            }
        }
 
        public async Task TryExecAsync(int i)
        {
            using (var scope = await _gate.TryAcquireAsync().ConfigureAwait(false))
            {
                if (!scope.Acquired)
                    Console.WriteLine("Potential deadlock");
                else
                {
                    Console.Write($"#{i}");
                    await Task.Delay(1500).ConfigureAwait(false);
                    Console.Write($"{i}# ");
                }
            }
        }
       
    }
}
