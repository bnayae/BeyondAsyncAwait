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

        private static async void BadIdeaAsync()
        {
            await Task.Delay(10);
            throw new NotSupportedException("Should it crash the application?");
        }

        private static async Task GoodIdeaAsync()
        {
            await Task.Delay(10);
            throw new NotSupportedException("Should it crash the application?");
        }

        // like command in WPF or event handling in general
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
    }
}