using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _02._1_Async_Download
{
    class Program
    {
        private const string URL = "https://source.unsplash.com/random/1200x1200";

        static void Main(string[] args)
        {
            Task t = ProcessAsync();

            while (!t.IsCompleted)
            {
                Console.Write(".");
                Thread.Sleep(10);
            }
            Console.WriteLine("Done");
            Console.ReadKey();
        }

        private static async Task ProcessAsync()
        {
            Task<byte[]> t1 = DownloadAsync(URL);
            Task<byte[]> t2 = DownloadAsync(URL);
            byte[][] images = await Task.WhenAll(t1, t2);
            int i = 0;
            foreach (byte[] data in images)
            {
                await SaveAsync($"Image {i++}.jpg", data);
            }
        }
        private static async Task<byte[]> DownloadAsync (string url)
        {
            using (var http = new HttpClient())
            {
                byte[] data = await http.GetByteArrayAsync(url);
                return data;
            }
        }
        private static async Task SaveAsync (string name, byte[] data)
        {
            using (var fs = new FileStream(name, FileMode.Create, FileAccess.Write, FileShare.None, 4096, FileOptions.Asynchronous))
            {
                await fs.WriteAsync(data, 0, data.Length);
            }
        }
    }
}
