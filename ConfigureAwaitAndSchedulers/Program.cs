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
        private static TaskScheduler _scheduler = new PoolScheduler(2);
        static void Main(string[] args)
        {
            Write("Main");
            Task t = ExecAsync();

            Console.ReadKey();
        }

        private static async Task ExecAsync()
        {
            Write("#1");
            await Task.Run(() => Write("#2 Run"));
            await Task.Factory.StartNew(async () =>
                        {
                            Write("#3 [Start New + Scheduler]");

                            // Configure Await False will forget the TaskScheduler.Current
                            await Task.Delay(1).ConfigureAwait(false);

                            Write("#4 Factory (after await)");
                            await Task.Run(() => Write("#5 [Run]: "));
                            await Task.Factory.StartNew(() => Write("#6 [Start New]: "));
                       },
                                CancellationToken.None, TaskCreationOptions.None,
                                _scheduler)
                                .Unwrap();
            Write("#7 After Factory");
        }

        private static void Write(string title)
        {
            title = title.PadRight(30, ' ');
            //TaskScheduler.Current
            var t = Thread.CurrentThread;
            Console.WriteLine($"{title} [{t.ManagedThreadId}]: On = [{t.Name ?? "Default"}]");
        }
    }
}
