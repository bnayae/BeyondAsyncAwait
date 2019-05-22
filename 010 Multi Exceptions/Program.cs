using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _010_Multi_Exceptions
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                Task t1 = Task.Run(() =>
                {
                    Console.WriteLine("Exec A");
                    throw new NullReferenceException("A");
                });
                Task t2 = Task.Factory.StartNew(() =>
                {
                    Console.WriteLine("Exec B");
                    throw new ArgumentException("B");
                });

                await Task.WhenAll(t1, t2);
                //Task t = Task.WhenAll(t1, t2);
                //Task _ = t.ContinueWith(c => Console.WriteLine(
                //            $"%%%%  {c.Exception?.GetType().Name} %%%%"), TaskContinuationOptions.OnlyOnFaulted);
                //await t;
                //await t.ThrowAll(); //.ContinueWith(c => throw c.Exception);

            }
            catch (AggregateException ex)
            {
                Console.WriteLine($"######\r\n{ex}\r\n######");
            }
            catch (NullReferenceException ex)
            {
                Console.WriteLine(ex);
            }

            catch (ArgumentException ex)
            {
                Console.WriteLine(ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"------- \r\n {ex}");
            }

            Console.ReadKey();

        }
    }
}
