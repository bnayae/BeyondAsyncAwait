using AsyncRelayMediator.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Threading.Tasks;

namespace AsyncRelayMediator.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WindowWidth = 40;
            Console.WindowHeight = 8;
            if (args.Length != 2)
                throw new ArgumentOutOfRangeException("Expecting single argument");

            string name = args[0];
            int port = int.Parse(args[1]);
            var channel = new TcpChannel(port);
            string relativeEndpoint = $"{name}.rem";
            string absEndpoint = $"tcp://localhost:{port}/{relativeEndpoint}";
            ChannelServices.RegisterChannel(channel, ensureSecurity: false);
            RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(Addressable), relativeEndpoint, WellKnownObjectMode.SingleCall);
            Addressable.Bind(name, absEndpoint);

            #region User route selection

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[{name}] Registered");
            Console.ResetColor();
            Console.WriteLine($"Set a route (press Enter at the end)");
            var route = new List<string>();
            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                    break;
                var selection = key.KeyChar.ToString().ToUpper();
                route.Add(selection);
                Console.Write($"{selection} -> ");
            } 
            Console.WriteLine(name);

            #endregion // User route selection

            Task _ = ExecRouteAsync(name, route);
            Console.ReadLine();
        }

        private static async Task ExecRouteAsync(
            string name, IEnumerable<string> route)
        {
            Console.WriteLine("Sending ...");
            await Addressable.SendAsync(
                            "Start", route);
            Console.WriteLine("Done");

        }
    }
}
