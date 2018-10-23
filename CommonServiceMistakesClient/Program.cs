using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CommonServiceMistakesClient
{
    class Program
    {
        private const int MAX = 200;

        private static readonly (string Title, string Server)[] SERVERS =
        {
            ("Core on IIS Express", "localhost:56368"),
            ("Core on exe", "localhost:56369"),
            ("Core on Docker", "localhost:32769"),
            ("Fx on IIS Express", "localhost:50257"),
            ("Fx Deadlock on IIS Express", "http://localhost:50257/api/Pitfall/deadlock"),
            ("Fx Deadlock Getaway on IIS Express", "http://localhost:50257/api/Pitfall/deadlock-getaway"),
        };

        private static readonly (string Title, string Url)[] END_POINTS =
        {
            ("Right", "http://{0}/api/sample/right/"),
            ("Needless", "http://{0}/api/sample/needless/"),
            ("Stupid", "http://{0}/api/sample/stupid/"),
            ("Silly", "http://{0}/api/sample/silly/ "),
            ("Sync", "http://{0}/api/sample/sync/"),
        };

        private static string _server;

        static void Main(string[] args)
        {
            int selection = ServerSelectionAndWarmup();

            switch (selection)
            {
                case 4:
                case 5:
                    var info = SERVERS[selection];
                    Task _ = DeadlockAsync(info);
                    break;
                default:
                    PoolBehavior();
                    break;
            }

            Console.ReadKey();
        }

        #region PoolBehavior

        private static void PoolBehavior()
        {
            for (int i = 0; i < 10; i++)
            {
                foreach (var endPointTemplate in END_POINTS)
                {
                    string endPoint = string.Format(endPointTemplate.Url, _server);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"==================  Start {endPointTemplate.Title} ==================");
                    Console.ResetColor();
                    Console.WriteLine("Press any key for the next test...");
                    Console.ReadKey(true);

                    Task test = CallAsync(endPoint, MAX);
                    test.Wait();
                }
            }
        }


        #endregion // PoolBehavior

        #region DeadlockAsync

        private static async Task DeadlockAsync((string Title, string Url) info)
        {
            Console.WriteLine(info.Title);
            try
            {
                using (var http = new HttpClient() { Timeout = TimeSpan.FromSeconds(15) })
                {
                    Console.WriteLine("Calling ....");
                    string result = await http.GetStringAsync(info.Url).ConfigureAwait(false);
                    Console.WriteLine(result);
                }
            }
            catch (OperationCanceledException)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("Timeout");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine(ex);
                Console.ResetColor();
            }
        }

        #endregion // DeadlockAsync

        #region ServerSelectionAndWarmup

        private static int ServerSelectionAndWarmup()
        {
            Console.WriteLine("Select Server");
            for (int i = 0; i < SERVERS.Length; i++)
            {
                string server = SERVERS[i].Title;
                Console.WriteLine($"    {i}. {server}");
            }
            char c = Console.ReadKey(true).KeyChar;
            int selection = int.Parse(c.ToString());
            _server = SERVERS[selection].Server;

            if (selection >= 4)
                return selection;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Targeting: {SERVERS[selection].Title}");
            Console.ResetColor();

            Console.WriteLine("Warm-up");
            Wormup();


            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Targeting: {SERVERS[selection].Title}");
            Console.ResetColor();
            return selection;
        }

        #endregion // ServerSelectionAndWarmup

        #region CallAsync

        private static async Task CallAsync(string root, int times)
        {
            var sw = Stopwatch.StartNew();
            using (var http = new HttpClient(new HttpClientHandler() { MaxConnectionsPerServer = 1000 }))
            {
                var tasks = from i in Enumerable.Range(0, times)
                            let url = $"{root}{i}"
                            select InvokeAsync(http, url);
                string[] responses = await Task.WhenAll(tasks).ConfigureAwait(false);
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

        #endregion // CallAsync

        #region InvokeAsync

        private static async Task<string> InvokeAsync(HttpClient http, string url)
        {
            var latency = Stopwatch.StartNew();
            Console.Write("-");
            string response = await http.GetStringAsync(url).ConfigureAwait(false);
            latency.Stop();
            Console.Write($"{latency.Elapsed.TotalSeconds:0.0}, ");
            //Console.Write("|");
            //Console.Write($"{response},");
            return response;
        }

        #endregion // InvokeAsync

        #region Worm-up

        private static void Wormup()
        {
            while (true)
            {
                try
                {
                    var warmups = END_POINTS
                        .Select(m => string.Format(m.Url, _server))
                        .Select(endPoint => CallAsync(endPoint, 1));
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

        #endregion // Worm-up
    }
}