using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bnaya.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Avoid async Void!!!");

            BadIdeaAsync();
            //Task _ = GoodIdeaAsync();
            //WhenYouMustAsync();

            Console.WriteLine("Still working");
            Thread.Sleep(1000);
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
            async Task LocalAsync() // C# 7 local function
            {
                await Task.Delay(10);
                throw new NotSupportedException("Should it crash the application?");
            }
            Task _ = LocalAsync();
        }

        #endregion // WhenYouMustAsync
    }
}