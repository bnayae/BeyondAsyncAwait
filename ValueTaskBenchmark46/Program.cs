using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// http://benchmarkdotnet.org/index.htm
// http://benchmarkdotnet.org/Configs/Exporters.htm

namespace Bnaya.Samples
{
    static class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<AsyncBenchmark>();
            Console.WriteLine(summary);

        }
    }
}
