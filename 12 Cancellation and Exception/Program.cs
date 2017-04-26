using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bnaya.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start");
            //CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(1.5));
            CancellationTokenSource cts = new CancellationTokenSource();

            Task _ = ExecAsync(cts.Token);

            Thread.Sleep(1000);
            Console.WriteLine("Canceling");

            RiskyCancellation(cts);
            //SaferCancellation(cts);

            Console.WriteLine("Done");

            Console.ReadKey();
        }

        private static async Task ExecAsync(CancellationToken token)
        {
            await Task.Delay(100);
            // you better check each and every registration
            token.Register(() => throw new InvalidOperationException("Shit can happen"));

        }

        private static void RiskyCancellation(CancellationTokenSource cts)
        {
            cts.Cancel();
        }

        private static void SaferCancellation(CancellationTokenSource cts)
        {
            try
            {
                cts.Cancel();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Format(false));
            }
        }
    }
}