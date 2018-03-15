using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
#pragma warning disable Await1 // Method is not configured to be awaited

namespace Bnaya.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(Thread.CurrentThread.ManagedThreadId);

            var setting = new Setting();
            var logic = new Logic(setting);
            Task  t = CallAsync(logic);
            while (!t.IsCompleted)
            {
                Console.Write(".");
                Thread.Sleep(50);
            }
            t = CallAsync(logic);
            Console.ReadKey();
        }

        private static async Task CallAsync(Logic logic)
        {
            int i = await logic.ExecAsync();
            Console.WriteLine(i);
        }
    }
}
