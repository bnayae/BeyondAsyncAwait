using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace _001_From_Sync_To_async
{
    class Program
    {
        private const string URL = "https://vetstreet.brightspotcdn.com/dims4/default/5b3ffe7/2147483647/thumbnail/180x180/quality/90/?url=https%3A%2F%2Fvetstreet-brightspot.s3.amazonaws.com%2F8e%2F4e3910c36111e0bfca0050568d6ceb%2Ffile%2Fhub-dogs-puppy.jpg";
        static void Main(string[] args)
        {
            Console.WriteLine("Start");
            //DownloadSync();
            //DownloadEvent();
            //DownloadContinue();
            Task fireForget = DownloadAsync();
            fireForget.ContinueWith(c => Console.WriteLine(c.Exception),
                                        TaskContinuationOptions.OnlyOnFaulted);


            Console.WriteLine("Read Key");
            Console.ReadKey();
        }

        private static void DownloadSync()
        {
            using (var wc = new WebClient())
            {
                byte[] image = wc.DownloadData(URL);
                using (var fs = File.OpenWrite($"Sync-{DateTime.Now:HH_mm_ss}.jpg"))
                {
                    fs.Write(image, 0, image.Length);
                }
            }
            Console.WriteLine("Sync Ready");
        }

        private static async Task DownloadAsync()
        {
            //var l = new List<int>();
            using (var wc = new WebClient())
            {
                //End of Main Thread (return Task)
                byte[] image = await wc.DownloadDataTaskAsync(new Uri(URL));
                //using (var fs = File.OpenWrite($"Sync-{DateTime.Now:HH_mm_ss}.jpg"))
                using (var fs = new FileStream($"Async-{DateTime.Now:HH_mm_ss}.jpg", 
                                                FileMode.Create, FileAccess.Write, FileShare.None, 4096, 
                                                FileOptions.Asynchronous))
                {
                    // End of Thread Pool Execution (scheduled by the TaskScheduler)
                    await fs.WriteAsync(image, 0, image.Length);
                }
            }
            Console.WriteLine("Async Ready");
            // End of Thread Pool Execution (scheduled by the TaskScheduler)
            // Task state change to completed
        }

        private static void DownloadEvent()
        {
            var wc = new WebClient();
            //using (var wc = new WebClient())
            //{
            wc.DownloadDataCompleted += (s, e) =>
            {
                byte[] image = e.Result;
                using (var fs = File.OpenWrite($"Event-{DateTime.Now:HH_mm_ss}.jpg"))
                {
                    fs.Write(image, 0, image.Length);
                }
                wc.Dispose();
                Console.WriteLine("Event Ready");
            };
            wc.DownloadDataAsync(new Uri(URL));
            //}
        }

        private static void DownloadContinue()
        {
            var wc = new WebClient();
            //using (var wc = new WebClient())
            //{
            Task<byte[]> imageTask = wc.DownloadDataTaskAsync(new Uri(URL));
            imageTask.ContinueWith(c =>
            {
                byte[] image = c.Result;
                using (var fs = File.OpenWrite($"Continue-{DateTime.Now:HH_mm_ss}.jpg"))
                {
                    fs.Write(image, 0, image.Length);
                }
                wc.Dispose();
                Console.WriteLine("Continue Ready");
            });
            //}
        }
    }
}
