using AsyncRelayMediator.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncRelayMediator.Service
{
    public class RelayService : MarshalByRefObject, IRelayService
    {
        private static readonly Type ADDRESSABLE_CONTRACT = typeof(IAddressable);
        private static readonly ConcurrentDictionary<string, string> _nameToPort = new ConcurrentDictionary<string, string>();

        public void Register(string key, string url)
        {
            _nameToPort.TryAdd(key, url);
        }

        public void SendViaRelay(Message message)
        {
            Task t = SendViaRelayAsync(message);
        }

        public async Task SendViaRelayAsync(Message message)
        {
            var routeTo = _nameToPort[message.NextDestination];
            IAddressable proxy = (IAddressable)RemotingServices.Connect(
                                                ADDRESSABLE_CONTRACT, routeTo);

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("confirm communication to ");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"[{message.NextDestination}]");
            Console.ResetColor();
            while (!Console.KeyAvailable) // emulate user data validation 
                await Task.Delay(40);
            Console.ReadKey(true);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Routing to: {message.NextDestination}");
            Console.ResetColor();

            proxy.Send(message);
        }
    }
}
