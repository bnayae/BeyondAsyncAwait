using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContinuationSample
{
    class Program
    {
        static void Main(string[] args)
        {
            Task<string> a = Task.Run(() =>
            {
                for (int i = 0; i < 10; i++)
                {
                    Console.Write("X");
                }
                return "X";
            });
            Task<string> b = Task.Run(() =>
            {
                for (int i = 0; i < 10; i++)
                {
                    Console.Write("Y");
                }
                return "Y";
            });

            Task.Factory.ContinueWhenAll(new[] { a , b}, 
                ts => Console.WriteLine($": {ts[0].Result}, {ts[1].Result}"));

            Console.ReadKey();
        }
    }
}
