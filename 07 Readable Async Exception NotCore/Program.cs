using AsyncFriendlyStackTrace;
using System;
using System.Threading.Tasks;
#pragma warning disable Await1 // Method is not configured to be awaited

namespace Bnaya.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            //Task _ = DefaultAsync();
            //Task _ = FormatAsync();
            //Task _ = FriendlyStackAsync();
            //Task _ = DefaultMultiAsync();
            //Task _ = FormatMultiAsync();
            //Task _ = FriendlyStackMultiAsync();
            Console.ReadKey();
        }

        private static async Task DefaultAsync()
        {
            try
            {
                await Step1Async();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static async Task FormatAsync()
        {
            try
            {
                await Step1Async();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Format());
            }
        }

        private static async Task FriendlyStackAsync()
        {
            try
            {
                await Step1Async();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToAsyncString());
            }
        }

        private static async Task DefaultNonSequentialAsync()
        {
            try
            {
                await NonSequentialRootAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static async Task FormatNonSequentialAsync()
        {
            try
            {
                await NonSequentialRootAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Format());
            }
        }

        private static async Task FriendlyStackMultiAsync()
        {
            try
            {
                await NonSequentialRootAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToAsyncString());
            }
        }

        private static async Task NonSequentialRootAsync()
        {
            await Task.Delay(1);
            await NonSequentialSplitAsync();
        }

        private static async Task NonSequentialSplitAsync()
        {
            var t1 = Task.Run(() => throw new ArgumentException("Other Error"));
            var t2 = Step1Async();
            await Task.WhenAll(t1, t2).ThrowAll();
        }

        private static async Task Step1Async()
        {
            await Task.Delay(1);
            await Step2Async();
        }

        private static async Task Step2Async()
        {
            try
            {
                await Task.Delay(1);
                await Step3Async();
            }
            catch (Exception ex)
            {
                throw new NullReferenceException("in between", ex);
            }
        }

        private static async Task Step3Async()
        {
            await Task.Delay(1);
            await Step4Async();
        }

        private static async Task Step4Async()
        {
            await Task.Delay(1);
            throw new FormatException("Illegal");
        }
    }
}