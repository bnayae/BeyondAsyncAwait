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
    [RoutePrefix("api/sample")]
    public class SampleController : ApiController
    {
        private static int DELAY = 2000;
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
        public async Task<string> GetStupid(int i)
        {   // yes, this one is stupid (but sometimes stupid code can be spot at production ;-) 
            Debug.Write(".");
            Thread.Sleep(Delay); // represent IO call
            return $"#{i:00}";
        }

        // GET api/sample/silly/{i}
        [Route("silly/{i}")]
        [HttpGet]
        public Task<string> GetSilly(int i)
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
        public Task<string> GetNeedless(int i)
        {   // swapping one Thread from a pool with another is needless
            Debug.Write(".");
            return Task.Run(async () =>
            {
                await Task.Delay(Delay); // represent IO call
                return $"#{i:00}";
            });
        }

        // GET api/sample/right/{i}
        [Route("right/{i}")]
        [HttpGet]
        public async Task<string> GetRight(int i)
        {
            Debug.Write(".");
            await Task.Delay(Delay); // represent IO call
            return $"#{i:00}";
        }
    }
}
