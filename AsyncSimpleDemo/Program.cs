using System;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncSimpleDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start");
            Task t = ExecAsync();
            for (int i = 0; i < 20; i++)
            {
                Console.Write("-");
                Thread.Sleep(1);
            }
            while (!t.IsCompleted)
            {
                Console.Write("^");
            }

            Console.ReadKey();
        }

        private static async Task ExecAsync()
        {
            for (int i = 0; i < 30; i++)
            {
                Console.Write("@");
            }
            await NextAsync();
                for (int i = 0; i < 30; i++)
                {
                    Console.Write("*");
                }
        }

        private static async Task NextAsync()
        {
            for (int i = 0; i < 30; i++)
            {
                Console.Write(".");
            }
            await Task.Delay(50);
            for (int i = 0; i < 300; i++)
            {
                Console.Write("#");
            }
        }
    }
}
