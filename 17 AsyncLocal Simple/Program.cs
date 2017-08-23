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
        private static AsyncLocal<string> _contexts = new AsyncLocal<string>();

        static void Main(string[] args)
        {
            Task _ = Exec1Async("A", 10);
            _ = Exec1Async("B", 20);
            Console.ReadKey();
        }

        private static async Task Exec1Async(string s, int i)
        {
            _contexts.Value = i.ToString();
            await Task.Delay(10);
            await Exec2Async(s);
        }
        private static async Task Exec2Async(string s)
        {
            await Task.Delay(10);
            await Exec3Async(s);
            Console.WriteLine($"# {_contexts.Value}");
        }
        private static async Task Exec3Async(string s)
        {
            _contexts.Value += 1;
            await Task.Delay(10);
            await Exec4Async(s);
        }
        private static async Task Exec4Async(string s)
        {
            await Task.Delay(10);
            Console.WriteLine($"Context {_contexts.Value} for {s}");
        }
    }
}
