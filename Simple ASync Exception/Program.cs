using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple_ASync_Exception
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                Console.WriteLine("May be faulted");

                await Task.Run(() => //throw new IndexOutOfRangeException("A"))
                                    Console.WriteLine("B"));

                throw new ArgumentNullException("X");
                //Console.WriteLine("C");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            Console.ReadKey();
        }
    }
}
