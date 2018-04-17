using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _17_AsyncLocal_Simple
{
    class Program
    {

        static void Main(string[] args)
        {
            var service = new AsyncContext();
            //var service = new LegacyAsyncContext();
            //var service = new ThreadContext();
            var items = new[] { "1st", "2dn", "3rd", "4th", "5th", "6th", "7th", "8th", "9th" };
            for (int i = 1; i <= items.Length; i++)
            {
                Task _ = service.Exec1Async(items[i - 1], i);
            }
            Console.ReadKey();
        }
    }
}
