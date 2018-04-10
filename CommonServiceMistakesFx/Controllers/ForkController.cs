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
    [RoutePrefix("api/fork")]
    public class ForkController : ApiController
    {
        // GET http://localhost:50257/api/fork/false
        [Route("false")]
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
            Thread.Sleep(2000); // block the synchronization context
            // await Task.Delay(2000); // this won't block the synchronization context
        }

        // GET http://localhost:50257/api/fork/true
        [Route("true")]
        [HttpGet]
        public async Task<TimeSpan> GetTrueFork()
        {
            var sw = Stopwatch.StartNew();
            Task a = ExecAsync(1);
            Task b = ExecAsync(2);
            await Task.WhenAll(a, b); // really run in-parallel
            sw.Stop();
            return sw.Elapsed;
        }

        private async Task ExecAsync(int i)
        {
            await Task.Delay(1).ConfigureAwait(false);
            // return on non-synchronization context
            Thread.Sleep(2000);
        }
    }
}
