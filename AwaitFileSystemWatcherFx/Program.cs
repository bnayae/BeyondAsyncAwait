using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Bnaya.Samples
{
    class Program
    {
        static void Main(string[] args)
        {
            string listenOn = Path.GetFullPath("Data");
            var fsw = new FileSystemWatcher(listenOn);
            Console.WriteLine(fsw.Path);
            Task t = ListenOnceAsync(fsw);
            //Task t = ContinuesListenAsync(fsw, cts.Token);
            while (!t.IsCompleted)
            {
                Console.Write(".");
                Thread.Sleep(500);
            }


            Console.ReadKey();
        }

        private static async Task ListenOnceAsync(FileSystemWatcher fsw)
        {
            string result = await fsw; // will call the GetAwaiter extension method
            Console.WriteLine(result);
        }

        private static async Task ContinuesListenAsync(
            FileSystemWatcher fsw,
            CancellationToken ct)
        {
            try
            {
                ct.Register(() => fsw.Dispose());

                while (!ct.IsCancellationRequested)
                {
                    string result = await fsw;
                    Console.Clear();
                    Console.WriteLine(result);
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Canceled");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}