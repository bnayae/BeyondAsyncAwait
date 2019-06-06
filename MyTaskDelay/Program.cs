using System;
using System.Threading;
using System.Threading.Tasks;

namespace MyTaskDelay
{
    class Program
    {

        static void Main(string[] args)
        {
            Task t = MyTask.Delay(TimeSpan.FromSeconds(2));
            while (!t.IsCompleted)
            {
                Console.Write(".");
                Thread.Sleep(50);
            }
            Console.WriteLine("Done");
            Console.ReadKey();
        }
    }

    public static class MyTask
    {
        public static Task Delay(TimeSpan delay)
        {
            var tcs = new TaskCompletionSource<object>();
            var timer = new Timer(OnComplete, tcs, delay, TimeSpan.Zero);
            return tcs.Task;
        }

        private static void OnComplete(object state)
        {
            var tcs = (TaskCompletionSource<object>)state;
            tcs.TrySetResult(null);
        }
    }
}
