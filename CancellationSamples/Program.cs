using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace CancellationSamples
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var cts = new CancellationTokenSource();
            //Task.Run((Action)Work, cts.Token);
            await Task.Run(() => Work(cts.Token), cts.Token);
        }

        private static void Work(CancellationToken ct)
        {
            try
            {
                A(ct);
            }
            catch (OperationCanceledException)
            {
                Trace.WriteLine("Canceled");
            }
        }

        private static void A(CancellationToken ct)
        {
            B(ct);
        }

        private static void B(CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
        }
    }
}
