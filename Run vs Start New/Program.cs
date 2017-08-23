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
            //Task _ = ExecAsync();
            Task _ = ParentChild();

            Console.ReadKey();
        }
        private static async Task ExecAsync()
        {
            Console.Write("1 ");

            // Should be sequential
            await Task.Factory.StartNew(async () =>
            {
                await Task.Delay(1000);
                Console.Write("2 ");
            });//.Unwrap();
            Console.Write("3 ");
        }
 
        private static async Task ParentChild()
        {
            await Task.Factory.StartNew(() =>
            {
                Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(1000);
                    Console.Write(".");
                    Task.Factory.StartNew(() =>
                    {
                        Thread.Sleep(1000);
                        Console.Write("!");
                    }, TaskCreationOptions.AttachedToParent);
                }, TaskCreationOptions.AttachedToParent);
            });//, TaskCreationOptions.DenyChildAttach);
            Console.Write("X");
        }

   }
}