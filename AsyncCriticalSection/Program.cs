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
            var tasks = Enumerable.Range(0, 10)
                            .Select(i => f.ExecAsync(i));
            Task t = Task.WhenAll(tasks);

        }
    }
}