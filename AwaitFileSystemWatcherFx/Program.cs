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
            var cts = new CancellationTokenSource(TimeSpan.FromMinutes(1));


            var fsw = new FileSystemWatcher(Path.GetFullPath("Data"));
            Console.WriteLine(fsw.Path);
            Task t = ListenAsync(fsw, cts.Token);
            while (!t.IsCompleted)
            {
                Console.Write(".");
                Thread.Sleep(500);
            }


            Console.ReadKey();
        }

        private static async Task ListenAsync(
            FileSystemWatcher fsw,
            CancellationToken ct)
        {
            try
            {
                ct.Register(() => fsw.Dispose());

                while (!ct.IsCancellationRequested)
                {
                    string result = await fsw;
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