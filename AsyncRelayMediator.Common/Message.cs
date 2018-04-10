using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AsyncRelayMediator.Common
{
    [Serializable]
    public class Message
    {
        #region Ctor

        public Message(
            string data,
            string replyTo,
            IEnumerable<string> route)
            :this(Guid.NewGuid(), data, route)
        {
            if(!string.IsNullOrEmpty(replyTo))
                Route = Route.Enqueue(replyTo);
        }

        public Message(
            Guid routeId,
            string data,
            IEnumerable<string> route)
        {
            Route = ImmutableQueue.CreateRange(route.Select(m => m));
            RouteId = routeId;
            Data = data;
        }

        #endregion // Ctor

        public readonly Guid RouteId;
        public string NextDestination => Route?.Peek();
        [NonSerialized]
        public ImmutableQueue<string> Route;
        public readonly string Data;

        #region Serialization Magic

        public string[] _routeSerialization;

        [OnSerializing()]
        private void OnSerializing(StreamingContext context)
        {
            _routeSerialization = Route.ToArray();
        }

        [OnSerialized()]
        internal void OnSerialized(StreamingContext context)
        {
            _routeSerialization = null;
        }

        [OnDeserialized()]
        internal void OnDeserialized(StreamingContext context)
        {
            Route = ImmutableQueue.CreateRange(_routeSerialization);
            _routeSerialization = null;
        }

        #endregion // Serialization Magic
    }
}
