using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bnaya.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            Info("Main");
            Task t = CanBeTrickyAsync();
            //Task t = AlsoTrickyAsync();
            //Task t = ExpectedAsync();
            Info("Main");
            while (!t.IsCompleted)
            {
                Console.Write(".");
                Thread.Sleep(50);
            }
            
            Console.ReadKey();
        }

        private static async Task CanBeTrickyAsync()
        {
            Info("CanBeTrickyAsync Start");
            Thread.Sleep(2000);
            Info("CanBeTrickyAsync End");
        }

        private static async Task AlsoTrickyAsync()
        {
            Info("AlsoTrickyAsync Start");
            await Task.CompletedTask;
            Thread.Sleep(2000);
            Info("AlsoTrickyAsync End");
        }

        private static async Task ExpectedAsync()
        {
            Info("ExpectedAsync Start");
            await Task.Delay(2000);
            Info("ExpectedAsync End");
        }

        private static void Info(string title)
        {
            var trd = Thread.CurrentThread;
            Console.WriteLine($"{title} [{trd.ManagedThreadId}]");
        }
    }
}