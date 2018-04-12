using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.Net.Http;
using System.IO;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Net;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Effects;
using SixLabors.ImageSharp.Processing.Transforms;
using SixLabors.ImageSharp.Processing.Processors;
using SixLabors.ImageSharp.Processing.Overlays;
using SixLabors.ImageSharp.Processing.Dithering;
using SixLabors.ImageSharp.Processing.Binarization;
using SixLabors.ImageSharp.Processing.Convolution.Processors;
using SixLabors.ImageSharp.Processing.Filters;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Processing.Text;
using SixLabors.ImageSharp.Processing.Drawing;
using SixLabors.ImageSharp.Processing.Drawing.Brushes;
using SixLabors.ImageSharp.Processing.Drawing.Pens;
using SixLabors.Primitives;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
#pragma warning disable SG0029 // Potential XSS vulnerability
#pragma warning disable SG0005 // Weak random generator
namespace Bnaya.Samples.Controllers
{
    [RoutePrefix("api/imagemanip")]
    public class ImageManipController : ApiController
    {
        private const string URL_PATTERN = "https://source.unsplash.com/{0}x{0}/?{1}/";
        private const string URL_PATTERN_RND = "https://source.unsplash.com/random/{0}x{0}";
        private const string MEDIA_TYPE = "image/jpeg";

        #region GetMergeAsync

        // GET api/imagemanip/merge/{size}/{topic?}
        [Route("merge/{size}/{topic?}")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetMergeAsync(
            int size, string topic = null)
        {
            Task<Image<Rgba32>> t1 = GetWithFilterAsync(size, topic);
            Task<Image<Rgba32>> t2 = GetWithFilterAsync(size, topic);

            var results = await Task.WhenAll(t1, t2).ConfigureAwait(false);

            using (Image<Rgba32> img0 = results[0])
            using (Image<Rgba32> img1 = results[1])
            using (var imageProcessor = new Image<Rgba32>(size * 2, size))
            using (var outStream = new MemoryStream())
            {
                imageProcessor.Mutate(x => x
                                    .DrawImage(img0, 1)
                                    .DrawImage(img1, 1, new Point(size, 0)));
                imageProcessor.SaveAsJpeg(outStream);
                byte[] manipedImage = outStream.ToArray();
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new ByteArrayContent(manipedImage);
                response.Content.Headers.ContentType =
                    new MediaTypeHeaderValue(MEDIA_TYPE);
                return response;
            }
        }

        #endregion // GetMergeAsync

        #region GetWithFilterAsync

        public async Task<Image<Rgba32>> GetWithFilterAsync(
            int size,
            string topic = null)
        {
            Trace.WriteLine("Start");
            string url;
            if (string.IsNullOrEmpty(topic))
                url = string.Format(URL_PATTERN_RND, size);
            else
                url = string.Format(URL_PATTERN, size, topic);

            using (var http = new HttpClient())
            {
                var image = await http.GetByteArrayAsync(url);//.ConfigureAwait(false);
                Trace.WriteLine("Processing");

                Image<Rgba32> imageProcessor = Image.Load(image);
                imageProcessor.Mutate(x => x
                        .OilPaint()
                        .Grayscale());
                Trace.WriteLine("Processed");
                return imageProcessor;
            }
        }

        #endregion // GetWithFilterAsync
    }
}
