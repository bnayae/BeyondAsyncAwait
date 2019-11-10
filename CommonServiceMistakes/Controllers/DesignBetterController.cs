using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Diagnostics;
using System.Net.Http;
using System.IO;
using System.Net.Http.Headers;
using Microsoft.Extensions.Caching.Memory;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
#pragma warning disable SG0029 // Potential XSS vulnerability
#pragma warning disable SG0005 // Weak random generator
namespace Bnaya.Samples.Controllers
{
    [Route("api/[controller]")]
    public class DesignBetterController : Controller
    {
        // https://loremipsum.io/21-of-the-best-placeholder-image-generators/
        //private const string URL = "https://loremflickr.com/{0}/{0}";
        //private const string URL_PATTERN = "https://picsum.photos/{0}";
        //private const string URL_PATTERN = "http://lorempixel.com/{0}/{0}/animals/";
        private const string URL_PATTERN = "https://source.unsplash.com/{0}x{0}/?{1}/";
        private const string MEDIA_TYPE = "image/jpeg";

        private readonly IDownloaderValueAsync _downloader;
        private readonly IMemoryCache _cache;

        // Constructor injection
        public DesignBetterController(
            IDownloaderValueAsync downloader,
            IMemoryCache cache)
        {
            _downloader = downloader;
            _cache = cache;
        }

        // GET api/designbad/async/{size}/{topic}
        [Route("async/{size}/{topic}")]
        [HttpGet]
        public async ValueTask<IActionResult> GetAsync(int size, string topic)
        {
            string url = string.Format(URL_PATTERN, size, topic);
            if (_cache.TryGetValue<FileContentResult>(url, out var cachedResponse))
                return cachedResponse;

            byte[] image = await _downloader.DownloadAsync(url);
            var response = File(image, MEDIA_TYPE);

            #region _cache.Set(url, response)

            // Set cache options.
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                // Keep in cache for this time, reset time if accessed.
                .SetSlidingExpiration(TimeSpan.FromSeconds(30));

            // Save data in cache.
            _cache.Set(url, response, cacheEntryOptions);

            #endregion // _cache.Set(url, response)

            return response;
        }
    }
}
