using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Custom_Scheduler
{
    class Program
    {
        static void Main(string[] args)
        {
            int i = 0;
            while (true)
            {
                i++;
                int local = i;
                Task.Factory.StartNew(() => Console.WriteLine($"No scheduler {local}"));
                Task.Factory.StartNew(async () =>
                {
                    Console.WriteLine($"Start With scheduler {local}");
                    await Task.Delay(4000);
                    Console.WriteLine($"End With scheduler {local}");
                },
                                    CancellationToken.None, TaskCreationOptions.None,
                                    TimerScheduler.Default).Unwrap();
                Thread.Sleep(500);
            }
        }
    }
}
