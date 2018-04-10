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

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
#pragma warning disable SG0029 // Potential XSS vulnerability
#pragma warning disable SG0005 // Weak random generator
namespace Bnaya.Samples.Controllers
{
    [Route("api/[controller]")]
    public class DesignOkController : Controller
    {
        private const string URL_PATTERN = "https://source.unsplash.com/{0}x{0}/?{1}/";
        private const string MEDIA_TYPE = "image/jpeg";

        private readonly IDownloaderAsync _downloader;

        // Constructor injection
        public DesignOkController(IDownloaderAsync downloader)
        {
            _downloader = downloader;
        }

        // GET api/designbad/async/{size}/{topic}
        [Route("async/{size}/{topic}")]
        [HttpGet]
        public async Task<IActionResult> GetAsync(int size, string topic)
        {
            string url = string.Format(URL_PATTERN, size, topic);
            byte[] image = await _downloader.DownloadAsync(url).ConfigureAwait(false);
            var response = File(image, MEDIA_TYPE);
            return response;
        }
    }
}
