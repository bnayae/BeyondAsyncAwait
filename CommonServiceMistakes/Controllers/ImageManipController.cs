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
using SkiaSharp;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
#pragma warning disable SG0029 // Potential XSS vulnerability
#pragma warning disable SG0005 // Weak random generator
namespace Bnaya.Samples.Controllers
{
    [Route("api/[controller]")]
    public class ImageManipController : Controller
    {
        private const string URL_PATTERN = "https://source.unsplash.com/{0}x{0}/?{1}/";
        private const string MEDIA_TYPE = "image/jpeg";

        // GET api/designbad/async/{size}/{topic}
        [Route("async/{size}/{topic}")]
        [HttpGet]
        public async ValueTask<IActionResult> GetAsync(int size, string topic)
        {
            string url = string.Format(URL_PATTERN, size, topic);
            using (var http = new HttpClient())
            {
                var result = await http.GetStreamAsync(url).ConfigureAwait(false);
            }
            throw new NotImplementedException();
            canvas.DrawColor(SKColors.White);

            // decode the bitmap from the stream
            using (var stream = new SKManagedStream(fileStream))
            using (var bitmap = SKBitmap.Decode(stream))
            using (var paint = new SKPaint())
            {
                // create the image filter
                using (var filter = SKImageFilter.CreateBlur(5, 5))
                {
                    paint.ImageFilter = filter;

                    // draw the bitmap through the filter
                    canvas.DrawBitmap(bitmap, SKRect.Create(width, height), paint);
                }
            }
            //byte[] image = await _downloader.DownloadAsync(url);
            //var response = File(image, MEDIA_TYPE);
            //return response;
        }
    }
}
