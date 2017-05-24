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
            //CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));
            CancellationTokenSource cts = new CancellationTokenSource();

            Task _ = ListenToCancellationAsync(cts.Token);

            Thread.Sleep(1000);
            Console.WriteLine("Canceling");

            #region Risky Cancellation

            cts.Cancel();

            #endregion // Risky Cancellation

            #region Safer Cancellation

            //try
            //{
            //    cts.Cancel();
            //}
            //catch (Exception)
            //{
            //    // TODO: Log
            //}

            #endregion // Safer Cancellation

            Console.WriteLine("Done");

            Console.ReadKey();
        }

        #region ListenToCancellationAsync

        private static async Task ListenToCancellationAsync(CancellationToken token)
        {
            await Task.Delay(100);
            // you better check each and every registration
            token.Register(() => throw new InvalidOperationException("Shit can happen"));

        }

        #endregion // ListenToCancellationAsync        
  }
}