using System;
using System.Threading;
using System.Threading.Tasks;
#pragma warning disable Await1 // Method is not configured to be awaited

namespace Bnaya.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Avoid async Void!!!");

            BadIdeaAsync();
            //Task fireForget = GoodIdeaAsync();
            //WhenYouMustAsync();
            //WhenYouMust7Async();

            Console.WriteLine("Still working");
            //Thread.Sleep(1000);
            while (true)
            {
                Thread.Sleep(50);
                Console.Write(".");
            }
            Console.WriteLine("If you see this line you didn't crushed");
            Console.ReadKey();
        }

        #region BadIdeaAsync

        private static async void BadIdeaAsync()
        {
            await Task.Delay(10);
            throw new NotSupportedException("Should it crash the application?");
        }

        #endregion // BadIdeaAsync

        #region GoodIdeaAsync

        private static async Task GoodIdeaAsync()
        {
            await Task.Delay(10);
            throw new NotSupportedException("Should it crash the application?");
        }

        #endregion // GoodIdeaAsync

        #region WhenYouMustAsync

        // like event handling in general
        private static void WhenYouMustAsync()
        {
            // wrap it
            Task _ = LocalAsync();
        }

        private static async Task LocalAsync() // C# 7 local function
        {
            await Task.Delay(10);
            throw new NotSupportedException("Should it crash the application?");
        }

        #endregion // WhenYouMustAsync

        #region WhenYouMust7Async

        // like event handling in general
        private static void WhenYouMust7Async()
        {
            // wrap it
            Task _ = Local7Async();

            async Task Local7Async() // C# 7 local function
            {
                await Task.Delay(10);
                throw new NotSupportedException("Should it crash the application?");
            }
        }

        #endregion // WhenYouMust7Async
    }
}