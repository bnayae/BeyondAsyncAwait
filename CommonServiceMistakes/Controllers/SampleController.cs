using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Diagnostics;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
namespace CommonServiceMistakes.Controllers
{
    [Route("api/[controller]")]
    public class SampleController : Controller
    {
        private const int DELAY = 2000;

        // GET api/sample/sync/{i}
        [Route("sync/{i}")]
        [HttpGet]
        public string GetSync(int i)
        {
            Debug.Write(".");
            Thread.Sleep(DELAY); // represent IO call
            return $"#{i:00}";
        }

        // GET api/sample/stupid/{i}
        [Route("stupid/{i}")]
        [HttpGet]
        public async Task<string> GetStupid(int i)
        {   // yes, this one is stupid (but sometimes stupid code can be spot at production ;-) 
            Debug.Write(".");
            Thread.Sleep(DELAY); // represent IO call
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
                Thread.Sleep(DELAY); // represent IO call
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
                await Task.Delay(DELAY); // represent IO call
                return $"#{i:00}";
            });
        }

        // GET api/sample/right/{i}
        [Route("right/{i}")]
        [HttpGet]
        public async Task<string> GetRight(int i)
        {
            Debug.Write(".");
            await Task.Delay(DELAY).ConfigureAwait(false); // represent IO call
            return $"#{i:00}";
        }
    }
}
