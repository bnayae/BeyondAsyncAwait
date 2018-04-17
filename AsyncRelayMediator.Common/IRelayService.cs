using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsyncRelayMediator.Common
{
    public interface IRelayService
    {
        void Register(string key, string url);
        void SendViaRelay(Message message);
    }
}
