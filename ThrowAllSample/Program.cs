using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThrowAllSample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                Task a = Task.Run(async () =>
                {
                    await Task.Delay(1000).ConfigureAwait(false);
                    throw new ArgumentException("A");
                });
                Task b = Task.Run(async () =>
                {
                    await Task.Delay(500).ConfigureAwait(false);
                    throw new NullReferenceException("B");
                });
                await Task.WhenAll(a, b).ThrowAll();

            }
            catch (AggregateException exs)
            {
                // Console.WriteLine(exs.Format());
                foreach (var ex in exs.Flatten().InnerExceptions)
                {
                    Console.WriteLine(ex.Format());
                    Console.WriteLine("----------------------");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Format());
            }

            Console.ReadKey();

        }
    }
}
