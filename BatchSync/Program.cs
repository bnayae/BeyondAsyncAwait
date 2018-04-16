using System;
using System.IO;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace BatchSync
{
    static class Program
    {

        static void Main(string[] args)
        {
            var options = new GroupingDataflowBlockOptions { Greedy = false };
            var sync = new BatchBlock<(int, double)>(3, options);

            // the delegate returns value tuple (.MET 4.7 / standard 2.0)
            var chA = new TransformBlock<int, (int, double)>(
                (Func<int, (int, double)>)ChannelA);
            var chB = new TransformBlock<int, (int, double)>(
                (Func<int, (int, double)>)ChannelB);
            var chC = new TransformBlock<int, (int, double)>(
                (Func<int, Task<(int, double)>>)ChannelC);

            var presenter = new ActionBlock<(int, double)[]>((Action<(int, double)[]>)Present);

            chA.LinkTo(sync);
            chB.LinkTo(sync);
            chC.LinkTo(sync);
            sync.LinkTo(presenter);


            for (int i = 0; i < 20; i++)
            {
                chA.Post(i);
                chB.Post(i);
                chC.Post(i);
            }

            Console.ReadKey();
        }

        #region ChannelA

        private static (int, double) ChannelA(int input) => (input, input * 1.2);

        #endregion // ChannelA

        #region ChannelB

        private static (int, double) ChannelB(int input)
        {
            int calc = input;
            for (int i = 0; i < 10_000; i++)
            {
                if ((calc * calc) % 2 == 0)
                    calc += calc % 10;
                else
                    calc -= calc % 10;
            }
            return (input,calc);
        }

        #endregion // ChannelB

        #region ChannelC

        private static async Task<(int, double)> ChannelC(int input)
        {
            int calc = input;
            for (int i = 0; i < 10; i++)
            {
                int x = Environment.TickCount % 10 - 5;
                calc += x;
                await Task.Delay(100).ConfigureAwait(false);
            }
            return (input, calc); /* value tuple */
        }

        #endregion // ChannelC

        #region Present

        public static void Present((int correlate, double calc)[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                Console.SetCursorPosition(i * 10, Console.CursorTop);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(data[i].correlate);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($": {data[i].calc}");
            }
            Console.WriteLine();
        }

        #endregion // Present

        #region Message (nested)

        private class Message
        {
            public Message(int correlate, int input)
            {
                Input = input;
                Correlate = correlate;
            }

            public int Input { get; }
            public int Correlate { get; }
        }

        #endregion // Message (nested)
    }

    public class EndMessage
    {
        public double Origin { get; set; }
        public double ChannelA { get; set; }
        public double ChannelB { get; set; }
        public double ChannelC { get; set; }
    }
}
