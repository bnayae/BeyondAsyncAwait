using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace APM_vs_Task
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Start");
            Func<int> f = () => { Thread.Sleep(2000); return 42; };
            //ApmWithCallback(f);
            TaskWithCallback(f);
            Console.ReadKey();
        }

        private static void TaskWithCallback(Func<int> f1)
        {
            // APM call
            Task<int> t = Task.Run(f1);
            Task<string> t1 = t.ContinueWith(TaskCallback);
            for (int i = 0; i < 10; i++)
            {
                t1.ContinueWith(c => Console.WriteLine(c.Result));
            }
        }
        private static string TaskCallback(Task<int> c)
        {
            Console.WriteLine(c.Result);
            Thread.Sleep(3000);
            return "hi";
        }

        private static void ApmWithCallback(Func<int> f1)
        {
            // APM call
            f1.BeginInvoke(ApmCallback, null);
        }

        private static void ApmCallback(IAsyncResult ar)
        {
            var real = (AsyncResult)ar;
            var f2 = (Func<int>)real.AsyncDelegate;
            int i = f2.EndInvoke(ar);
            Console.WriteLine(i);
        }
    }
}
