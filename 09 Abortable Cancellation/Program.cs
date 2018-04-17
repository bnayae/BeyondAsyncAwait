using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

// Cancellation: https://msdn.microsoft.com/en-us/library/dd997364(v=vs.110).aspx
// Don't use Abort (also not included in .NET Core): 
//
//  https://github.com/dotnet/coreclr/pull/2342/commits/80670756034034cb32ccf385130a62991ed6374a


namespace Bnaya.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start");

            var cancel = new CancellationTokenSource(TimeSpan.FromSeconds(2));
            Task.Factory.StartNew(() => Exec(cancel.Token), TaskCreationOptions.LongRunning);

            Console.ReadKey();
        }

        private static void Exec(CancellationToken ct)
        {
            Thread t = Thread.CurrentThread;
            ct.Register(() => t.Abort());
            try
            {
                while (true)
                {
                    Console.Write(".");
                    Thread.Sleep(100);
                }
            }
            catch (ThreadAbortException)
            {
                Console.WriteLine("Abort");
            }
        }
    }
}
