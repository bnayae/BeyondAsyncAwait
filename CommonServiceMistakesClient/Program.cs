using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CommonServiceMistakesClient
{
    class Program
    {
        private const int MAX = 50;
        //private const int PORT = 56368; // Core
        private const int PORT = 50257; // FX
        private static readonly (string Title, string Url)[] END_POINTS =
        {
            ("Right", $"http://localhost:{PORT}/api/sample/right/"),
            ("Needless", $"http://localhost:{PORT}/api/sample/needless/"),
            ("Stupid", $"http://localhost:{PORT}/api/sample/stupid/"),
            ("Silly", $"http://localhost:{PORT}/api/sample/silly/ "),
            ("Sync", $"http://localhost:{PORT}/api/sample/sync/"),
        };

        static void Main(string[] args)
        {
            Console.WriteLine("Warm-up");

            Warmup();
            for (int i = 0; i < 10; i++)
            {
                foreach (var endPoint in END_POINTS)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"==================  Start {endPoint.Title} ==================");
                    Console.ResetColor();

                    Task test = CallAsync(endPoint.Url, MAX);
                    test.Wait();
                }
            }

            Console.ReadKey();
        }

        private static async Task CallAsync(string root, int times)
        {
            var sw = Stopwatch.StartNew();
            using (var http = new HttpClient())
            {
                var tasks = from i in Enumerable.Range(0, times)
                            let url = $"{root}{i}"
                            select InvokeAsync(http, url);
                string[] responses = await Task.WhenAll(tasks);
                sw.Stop();
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine($"\r\n\r\nDURATION: {sw.Elapsed.TotalSeconds:N3} seconds");
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Green;
                //Array.Sort(responses);
                Console.WriteLine(string.Join(", ", responses));
                Console.ResetColor();
                Console.WriteLine("\r\n");
            }

        }

        private static async Task<string> InvokeAsync(HttpClient http, string url)
        {
            Console.Write("-");
            string response = await http.GetStringAsync(url);
            //Console.Write("|");
            Console.Write($"{response},");
            return response;
        }

        private static void Warmup()
        {
            while (true)
            {
                try
                {
                    var warmups = END_POINTS.Select(endPoint => CallAsync(endPoint.Url, 1));
                    Task.WaitAll(warmups.ToArray());
                    break;
                }
                catch (AggregateException ex) when (ex.InnerException is HttpRequestException)
                {
                    Console.WriteLine("Make sure to start the target site");
                }
            }
            Console.Clear();
        }
    }
}