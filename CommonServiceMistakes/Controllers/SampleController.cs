using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Diagnostics;

// ISSUE: https://github.com/aspnet/Hosting/issues/1058

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
#pragma warning disable SG0029 // Potential XSS vulnerability
#pragma warning disable SG0005 // Weak random generator
namespace Bnaya.Samples.Controllers
{
    [Route("api/[controller]")]
    public class SampleController : Controller
    {
        private const int DELAY = 2000;
        private static int DELAY_RANGE = 5;
        private ThreadLocal<Random> _rnd = new ThreadLocal<Random>(() => new Random(Guid.NewGuid().GetHashCode()));

        private TimeSpan Delay => TimeSpan.FromMilliseconds(DELAY + _rnd.Value.Next(0, DELAY_RANGE));


        // GET api/sample/sync/{i}
        [Route("sync/{i}")]
        [HttpGet]
        public string GetSync(int i)
        {
            Debug.Write(".");
            Thread.Sleep(Delay); // represent IO call
            return $"#{i:00}";
        }

        // GET api/sample/stupid/{i}
        [Route("stupid/{i}")]
        [HttpGet]
        public async Task<string> GetStupidAsync(int i)
        {   // yes, this one is stupid (but sometimes stupid code can be spot at production ;-) 
            Debug.Write(".");
            Thread.Sleep(Delay); // represent IO call
            return $"#{i:00}";
        }

        // GET api/sample/silly/{i}
        [Route("silly/{i}")]
        [HttpGet]
        public Task<string> GetSillyAsync(int i)
        {   // Still blocking the thread pool 
            Debug.Write(".");
            return Task.Run(() =>
            {
                Thread.Sleep(Delay); // represent IO call
                return $"#{i:00}";
            });
        }

        // GET api/sample/needless/{i}
        [Route("needless/{i}")]
        [HttpGet]
        public Task<string> GetNeedlessAsync(int i)
        {   // swapping one Thread from a pool with another is needless
            Debug.Write(".");
            return Task.Run(async () =>
            {
                await Task.Delay(Delay).ConfigureAwait(false); // represent IO call
                return $"#{i:00}";
            });
        }

        // GET api/sample/right/{i}
        [Route("right/{i}")]
        [HttpGet]
        public async Task<string> GetRightAsync(int i)
        {
            await Task.Delay(Delay).ConfigureAwait(false);//.ConfigureAwait(false); // represent IO call
            return $"#{i:00}";
        }
    }
}
