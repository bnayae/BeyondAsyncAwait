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
            var sync = new BatchBlock<string>(3, options);

            // the delegate returns value tuple (.MET 4.7 / standard 2.0)
            var chA = new TransformBlock<int, string>(
                (Func<int, string>)ChannelA);
            var chB = new TransformBlock<int, string>(
                (Func<int, string>)ChannelB);
            var chC = new TransformBlock<int, string>(
                (Func<int, Task<string>>)ChannelC);

            var presenter = new ActionBlock<string[]>((Action<string[]>)Present);

            chA.LinkTo(sync);
            chB.LinkTo(sync);
            chC.LinkTo(sync);
            sync.LinkTo(presenter);


            for (int i = 1; i <= 20; i++)
            {
                chA.Post(i);
                chB.Post(i);
                chC.Post(i);
            }

            Console.ReadKey();
        }

        #region ChannelA

        private static string ChannelA(int input) => new string('@', input);

        #endregion // ChannelA

        #region ChannelB

        private static string ChannelB(int input) => new string('#', input);


        #endregion // ChannelB

        #region ChannelC

        private static async Task<string> ChannelC(int input)
        {
            await Task.Delay((input % 5) * 500).ConfigureAwait(false);
            return new string('%', input);
        }

        #endregion // ChannelC

        #region Present

        public static void Present(string[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                Console.Write(data[i]);
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
        public string Origin { get; set; }
        public string ChannelA { get; set; }
        public string ChannelB { get; set; }
        public string ChannelC { get; set; }
    }
}
