using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
#pragma warning disable Await1 // Method is not configured to be awaited

namespace CommonServiceMistakesFx.Controllers
{
    [RoutePrefix("api/deadlock")]
    public class DeadlockController : ApiController
    {
        // GET api/deadlock/blocked
        [Route("blocked")]
        [HttpGet]
        public int GetDeadlockSync()
        {
            //SynchronizationContext.Current
            Task<int> t = InContextAsync();
            var result = t.Result; // hold the synchronization context and cause dead-lock
            return result;
        }

        private async Task<int> InContextAsync()
        {
            await Task.Delay(500);
            // when synchronization context is available (single method usage at a time)
            // cannot get into the this line because the calling method
            // hold the synchronization context (.Result)
            return 43;
        }

        // GET api/deadlock/getaway
        [Route("getaway")]
        [HttpGet]
        public int GetDeadlockGetawaySync()
        {
            Task<int> t = OutOfContextsync();
            var result = t.Result;
            return result;
        }

        private async Task<int> OutOfContextsync()
        {
            await Task.Delay(500).ConfigureAwait(false); // avoid synchronization context
            return 43;
        }
    }
}
