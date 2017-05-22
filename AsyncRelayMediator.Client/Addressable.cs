using AsyncRelayMediator.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Threading.Tasks;
using static AsyncRelayMediator.Common.Constants;

namespace AsyncRelayMediator.Client
{
    public class Addressable : MarshalByRefObject, IAddressable
    {
        private static string _name;
        private static ConcurrentDictionary<Guid, TaskCompletionSource<string>> _messageMap = new ConcurrentDictionary<Guid, TaskCompletionSource<string>>();

        private readonly static IRelayService _relayProxy;

        #region Ctor

        static Addressable()
        {
            string relayAddress = $"tcp://localhost:{RELAY_PORT}/relay.rem";
            _relayProxy = (IRelayService)RemotingServices.Connect(typeof(IRelayService), relayAddress);
        }

        #endregion // Ctor

        #region SendAsync // this is the magic

        public static Task<string> SendAsync(
            string data,
            IEnumerable<string> route)
        {

            var message = new Message(data, _name, route);
            _relayProxy.SendViaRelay(message);

            TaskCompletionSource<string> completion = new TaskCompletionSource<string>();
            _messageMap.TryAdd(message.RouteId, completion);
            return completion.Task;
        }

        #endregion // SendAsync // this is the magic

        #region IAddressable.Send // handle the send

        // it's receive on this side
        void IAddressable.Send(Message message)
        {
            var route = message.Route.Dequeue(out var next);
            Console.ForegroundColor = ConsoleColor.Green;
            if (route.IsEmpty)
            {
                if (_messageMap.TryRemove(message.RouteId, out var tcs)) // release the task of the original call
                    tcs.TrySetResult(message.Data);
            }
            else
            {
                Console.WriteLine($"Forward to: {string.Join(" -> ", route)}");
                var fw = new Message(message.RouteId,
                                    $"{message.Data} -> {_name}", 
                                    route);
                _relayProxy.SendViaRelay(fw);
            }
            Console.ResetColor();
        }

        #endregion // IAddressable.Send // handle the send

        public static void Bind(string name, string address)
        {
            _name = name;
            _relayProxy.Register(name, address);
        }
    }
}
