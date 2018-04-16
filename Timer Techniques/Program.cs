using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Timer_Techniques
{
    class Program
    {
        private static Timer _tmr;
        static void Main(string[] args) 
        {
            _tmr = new Timer(s =>
            {
                Console.Write(Thread.CurrentThread.IsThreadPoolThread);
                Task _ = Local();
                async Task Local()
                {
                    Console.Write("#");
                    await Task.Delay(3000);
                   // throw new ArgumentException();
                    Console.Write(".");
                }
            }, null, 500, 500);

            Console.ReadKey();
        }



    }
}
