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
            //SynchronizationContext.Current
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

        // GET http://localhost:50257/api/pitfall/false-fork
        [Route("false-fork")]
        [HttpGet]
        public async Task<TimeSpan> GetFalseFork()
        {
            var sw = Stopwatch.StartNew();
            Task a = ExecNotAsync(1);
            Task b = ExecNotAsync(2);
            await Task.WhenAll(a, b); // not really run in-parallel
            sw.Stop();
            return sw.Elapsed;
        }

        private async Task ExecNotAsync(int i)
        {
            await Task.Delay(1);
            // return on the synchronization context
            Thread.Sleep(2000);

            // await Task.Delay(2000); // this one will work fine (free the context)
        }

        // GET http://localhost:50257/api/pitfall/true-fork
        [Route("true-fork")]
        [HttpGet]
        public async Task<TimeSpan> GetTrueFork()
        {
            var sw = Stopwatch.StartNew();
            Task a = ExecAsync(1);
            Task b = ExecAsync(2);
            await Task.WhenAll(a, b); // not really run in-parallel
            sw.Stop();
            return sw.Elapsed;
        }

        private async Task ExecAsync(int i)
        {
            await Task.Delay(1).ConfigureAwait(false);
            // return on the synchronization context
            Thread.Sleep(2000);

            // await Task.Delay(2000); // this one will work fine (free the context)
        }
    }
}
