using System;
using System.Threading.Tasks;

namespace Bnaya.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start");
            Task _ = ExecAsync();

            Console.ReadKey();
        }

        private static async Task ExecAsync()
        {
            Console.Write("0 ");
            await ShouldBeSequentialAsync();
            Console.Write("4 ");
        }

        private static async Task ShouldBeSequentialAsync()
        {
            Console.Write("1 ");
           
            await Task.Factory.StartNew(async () =>
            {
                await Task.Delay(1000);
                Console.Write("2 ");
            }).Unwrap();
            Console.Write("3 ");
        }
    }
}