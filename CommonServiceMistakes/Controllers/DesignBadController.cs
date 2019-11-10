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
    public class DesignBadController : Controller
    {
        // https://loremipsum.io/21-of-the-best-placeholder-image-generators/
        //private const string URL = "https://loremflickr.com/640/360";
        //private const string URL_PATTERN = "https://picsum.photos/1200";
        //private const string URL_PATTERN = "http://lorempixel.com/800/800/animals/";
        private const string URL_PATTERN = "https://source.unsplash.com/{0}x{0}/?{1}/";
        private const string MEDIA_TYPE = "image/jpeg";

        private readonly IDownloader _downloader;

        // Constructor injection
        public DesignBadController(IDownloader downloader)
        {
            _downloader = downloader;
        }

        // GET api/designbad/sync/{size}/{topic}
        [Route("sync/{size}/{topic}")]
        [HttpGet]
        public IActionResult Get(int size, string topic)
        {
            string url = string.Format(URL_PATTERN, size, topic);
            byte[] image = _downloader.Download(url);

            var response = File(image, MEDIA_TYPE);
            return response;
        }

        // GET api/designbad/async/{size}/{topic}
        [Route("async/{size}/{topic}")]
        [HttpGet]
        public Task<IActionResult> GetAsync(int size, string topic)
        {
            string url = string.Format(URL_PATTERN, size, topic);
            byte[] image = _downloader.Download(url);
            var response = File(image, MEDIA_TYPE);
            return Task.FromResult<IActionResult>(response); // not very helpful
        }
    }
}
