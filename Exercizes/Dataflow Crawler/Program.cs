using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace _008_Transform_Many_and_Broadcast
{
    class Program
    {
        static void Main(string[] args)
        {
            var b1 = new TransformBlock<int, string>(i => new string((char)('a' + i), i));
            var b2 = new TransformManyBlock<string, char>(i => i);
            var b3 = new ActionBlock<char>(async c =>
            {
                await Task.Delay(1000).ConfigureAwait(false);
                Console.Write($"{c}, ");
            });
            var b4 = new ActionBlock<char>(async c =>
            {
                await Task.Delay(500).ConfigureAwait(false);
                Trace.Write($"{c}, ");
            });
            var b5 = new BroadcastBlock<char>(i => i);

            b1.LinkTo(b2);
            b2.LinkTo(b5);
            b5.LinkTo(b3);
            b5.LinkTo(b4);

            for (int i = 1; i <= 10; i++)
            {
                b1.Post(i);
            }

            Console.WriteLine($@"
b1: in {b1.InputCount} out {b1.OutputCount}
b2: in {b2.InputCount} out {b2.OutputCount} 
b3: in {b3.InputCount}
b4: in {b4.InputCount}
");
            Console.ReadKey();
        }
    }
}
