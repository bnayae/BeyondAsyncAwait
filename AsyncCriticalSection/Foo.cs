using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Bnaya.Samples
{
    public class Foo
    {
        private readonly static TimeSpan TIMEOUT = TimeSpan.FromMinutes(5);
        private readonly SemaphoreSlim _gate = new SemaphoreSlim(1);

        public async Task ExecAsync(int i)
        {
            if (!await _gate.WaitAsync(TIMEOUT).ConfigureAwait(false))
                throw new InvalidOperationException  ("Deadlock");
            try
            {
                Console.Write($"#{i}");
                await Task.Delay(1500).ConfigureAwait(false);
                Console.Write($"{i}# ");
            }
            finally
            {
                _gate.Release();
            }
        }


        public async Task TryExecAsync(int i)
        {
            using (var scope = await _gate.TryAcquireAsync(TimeSpan.FromSeconds(10)).ConfigureAwait(false))
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
