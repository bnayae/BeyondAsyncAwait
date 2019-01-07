using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CaptureVariablePitfall
{
    class Program
    {
        static void Main(string[] args)
        {
            for (int i = 0; i < 10; i++)
            {
                int local = i;
                ThreadPool.QueueUserWorkItem(state =>
                {
                    Thread.Sleep(20);
                    Console.Write($"{local}, ");

                }, null);
            }

            Console.ReadKey();
        }
    }
}
