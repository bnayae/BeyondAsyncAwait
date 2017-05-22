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
            Console.Write($"-{i}- ");
            await Task.Delay(500);
            Console.Write($".{i}. ");
            if (!await _gate.WaitAsync(TIMEOUT))
                throw new InvalidOperationException  ("Deadlock");
            try
            {
                Console.Write($"%{i}% ");
                await Task.Delay(500);
                Console.Write($"#{i}# ");
            }
            finally
            {
                _gate.Release();
            }
            Console.Write($"*{i}* ");
        }
        
    }
}
