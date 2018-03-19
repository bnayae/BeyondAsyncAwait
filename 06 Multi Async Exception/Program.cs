using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bnaya.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            Task fireForget = RunMultiAsync();

            Console.ReadKey();
        }
        private static async Task RunMultiAsync()
        {
            try
            {
                Console.WriteLine("Start");
                Task t1 = Task.Run(async () => 
                {
                    await Task.Delay(3000);
                    throw new IndexOutOfRangeException("Y");
                });
                Task t2 = Task.Run(() => throw new ArgumentOutOfRangeException("X"));
                await Task.WhenAll(t1, t2).ThrowAll();
                            //.ContinueWith(c =>
                            //{
                            //    //throw new NullReferenceException("Z");
                            //    if (c.Exception != null)
                            //        throw c.Exception;
                            //});
                await Task.Factory.StartNew(() => { });
            }
            catch (AggregateException ex)
            {
                Console.WriteLine($"### {ex}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static async Task RunAsync()
        {
            try
            {

                await Task.Delay(2);
                
                await Task.Run(() => { });
                await Task.Factory.StartNew(() => { });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        //private static Task RunContAsync()
        //{
        //    try
        //    {

        //        X();
        //        Task t = AAsync();
        //        t.ContinueWith(c => { Console.WriteLine(c.Exception) }, TaskContinuationOptions.OnlyOnFaulted);
        //        Task t1 = t.ContinueWith(c => { }, TaskContinuationOptions.OnlyOnRanToCompletion);
        //        Task t2 = t1.ContinueWith(c => { });
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}

        private static void X()
        {
        }

        private static Task AAsync() => Task.Delay(200);
    }
}
