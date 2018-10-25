using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Bnaya.Samples
{
    class Program
    {
        private const string URL = "https://source.unsplash.com/300x300/?dog/";
        private static ActionBlock<string> _throttle;

        static void Main(string[] args)
        {
            if (!Directory.Exists("Images"))
                Directory.CreateDirectory("Images");
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(8));
            //cts.Cancel();
            var options = new ExecutionDataflowBlockOptions
                                {
                                     MaxDegreeOfParallelism = 3,
                                     CancellationToken = cts.Token,                                     
                                };
            _throttle = new ActionBlock<string>(ProcessAsync, options);
            //for (int i = 0; i < 100_000; i++)
            //{
            //    _throttle.Post(URL);
            //}
            Parallel.For(0, 100_000, i => _throttle.Post(URL));
            //var tasks = Enumerable.Range(0, 100_000).AsParallel().Select(i => _throttle.Post(URL));//.ToArray();
            //Task.WhenAll(tasks);
            while (!_throttle.Completion.IsCompleted)
            {
                Console.Write(".");
                Thread.Sleep(50);
            }
            Console.WriteLine(_throttle.Completion.Status);
            var exs = _throttle.Completion.Exception;
            if (exs != null)
            {
                foreach (var ex in exs.Flatten().InnerExceptions)
                {
                    Console.WriteLine(ex.GetType().Name);
                }
            }
            Console.ReadKey();
        }

        private static async Task ProcessAsync(string url)
        {
            Console.Write("X");
            try
            {
                byte[] data = await DownloadAsync(url);
                await SaveAsync(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            //if (Environment.TickCount % 3 == 0)
            //    throw new IndexOutOfRangeException();
            //if (Environment.TickCount % 3 == 1)
            //    throw new DivideByZeroException();
        }

        private static async Task<byte[]> DownloadAsync(string url)
        {
            using (var http = new HttpClient())
            {
                var data = await http.GetByteArrayAsync(url);
                return data;
            }
        }
        private static async Task SaveAsync(byte[] data)
        {
            string path = $@"Images\{Guid.NewGuid().ToString("N")}.jpg";
            using (var fs = new FileStream(path,
                                    FileMode.Create, FileAccess.Write, FileShare.None, 40096, FileOptions.Asynchronous))
            {
                await fs.WriteAsync(data, 0, data.Length);
            }
        }

    }
}
