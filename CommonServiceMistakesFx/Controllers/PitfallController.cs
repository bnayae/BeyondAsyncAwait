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

namespace CommonServiceMistakesFx.Controllers
{
    [RoutePrefix("api/pitfall")]
    public class PitfallController : ApiController
    {
        // GET api/pitfall/deadlock
        [Route("deadlock")]
        [HttpGet]
        public int GetDeadlockSync()
        {
            Task<int> t = InContextAsync();
            var result = t.Result;
            return result;
        }

        private async Task<int> InContextAsync()
        {
            await Task.Delay(500);
            // when synchronization context is available (single method usage at a time)
            return 43;
        }

        // GET api/pitfall/deadlock-getaway
        [Route("deadlock-getaway")]
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
