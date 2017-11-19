using System;
using System.Linq;
using System.Threading.Tasks;

namespace Bnaya.Samples
{
    class Program
    {
        
        static void Main(string[] args)
        {
            var f = new Foo();
            //var f = new FooBetter();
            var tasks = Enumerable.Range(0, 10)
                            .Select(f.ExecAsync);
            Task t = Task.WhenAll(tasks);
            try
            {
                t.Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Format());
            }
            Console.WriteLine("\r\n=================================");

            tasks = Enumerable.Range(0, 10)
                            .Select(i => f.TryExecAsync(i));
            t = Task.WhenAll(tasks);
            t.Wait();
            Console.ReadKey();
        }
    }
}