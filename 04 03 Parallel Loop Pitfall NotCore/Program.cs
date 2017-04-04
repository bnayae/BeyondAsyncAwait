using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Bnaya.Samples
{
    class Program
    {
        private const int COMPUTE_LEVEL = 10000000;

        static void Main(string[] args)
        {
            Console.WriteLine("Press any key to start");
            var data = GetBlockingData();
            Parallel.ForEach(data, item => Console.Write($"{item}, "));
        }

        private static IEnumerable<int> GetBlockingData()
        {
            while (true)
            {
                var c = Console.ReadKey(true).KeyChar;
                yield return (int)c;
            }
        }

        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoOptimization)]
        private static IEnumerable<int> GetComputeData()
        {
            for (int j = 0; j < int.MaxValue; j++)
            {
                for (int i = 0; i < COMPUTE_LEVEL; i++)
                {
                }
                yield return j;
            }
        }
    }
}