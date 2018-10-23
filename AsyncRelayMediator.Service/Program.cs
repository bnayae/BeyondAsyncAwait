using AsyncRelayMediator.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static AsyncRelayMediator.Common.Constants;

#pragma warning disable SG0001 // Potential command injection with Process.Start
#pragma warning disable CC0022 // Should dispose object
#pragma warning disable SCS0001 // Possible command injection

namespace AsyncRelayMediator.Service
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WindowWidth = 40;
            Console.WindowHeight = 8;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Relay Starting ...");
            Console.ResetColor();

            var channel = new TcpChannel(RELAY_PORT);
            ChannelServices.RegisterChannel(channel, ensureSecurity: false);
            RemotingConfiguration.RegisterWellKnownServiceType(
                                typeof(RelayService),
                                $"relay.rem",
                                WellKnownObjectMode.SingleCall);

            for (int i = 0; i < 4; i++)
            {
                var key = (char)('A' + i);
                var p = new Process();
                p.StartInfo.Arguments = $"{key} {RELAY_PORT + 1 + i}";
                p.StartInfo.FileName = "AsyncRelayMediator.Client.exe";
                p.Start();
            }

            Console.WriteLine("Registered");
            Thread.Sleep(Timeout.Infinite);
        }
    }
}
