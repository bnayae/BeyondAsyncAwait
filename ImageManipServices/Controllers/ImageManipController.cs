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

        #region GetBlurAsync

        // GET api/imagemanip/blur/{size}/{topic}/{blurStrength}
        [Route("blur/{size}/{topic}/{blurStrength}")]
        [HttpGet]
        public async ValueTask<IActionResult> GetBlurAsync(int size, string topic, int blurStrength)
        {
            using (var filter = SKImageFilter.CreateBlur(blurStrength, blurStrength))
            {
                var response = await GetWithFilterAsync(size, topic, filter);
                return response;
            }
        }

        #endregion // GetBlurAsync

        #region GetGrayscaleAsync

        // GET api/imagemanip/grayscale/{size}/{topic}/{contrast?}
        [Route("grayscale/{size}/{topic}/{contrast?}")]
        [HttpGet]
        public async ValueTask<IActionResult> GetGrayscaleAsync(int size, string topic, float contrast = 0.06F)
        {
            int halfSize = size / 2;
            var rotate = SKMatrix.MakeRotationDegrees(30, halfSize, halfSize);
            var scale = SKMatrix.MakeScale(1.2F, 1.2F, halfSize, halfSize);
            using (var filter = SKColorFilter.CreateHighContrast(
                true, SKHighContrastConfigInvertStyle.NoInvert, contrast))
            {
                var response = await GetWithFilterAsync(size, topic, colorFilter: filter);
                return response;
            }
        }

        #endregion // GetGrayscaleAsync

        #region GetRotateAsync

        // GET api/imagemanip/rotate/{size}/{topic}/{degrees}
        [Route("rotate/{size}/{topic}/{degrees?}")]
        [HttpGet]
        public async ValueTask<IActionResult> GetRotateAsync(int size, string topic, int degrees = 30)
        {
            int halfSize = size / 2;
            var rotate = SKMatrix.MakeRotationDegrees(degrees, halfSize, halfSize);
            var scale = SKMatrix.MakeScale(1.2F, 1.2F, halfSize, halfSize);
            using (var filter = SKImageFilter.CreateMatrix(rotate, SKFilterQuality.Medium))
            {
                var response = await GetWithFilterAsync(size, topic, filter);
                return response;
            }
        }

        #endregion // GetRotateAsync

        #region GetShaderAsync

        // GET api/imagemanip/shader/{size}/{topic}
        [Route("shader/{size}/{topic}")]
        [HttpGet]
        public async ValueTask<IActionResult> GetShaderAsync(int size, string topic)
        {
            int center = size / 2;
            var colors = new SKColor[] { new SKColor(0, 255, 90), new SKColor(50, 0, 255), new SKColor(255, 255, 0), new SKColor(0, 255, 255) };
            using (var sweep = SKShader.CreateSweepGradient(new SKPoint(center, center), colors, null))
            using (var turbulence = SKShader.CreatePerlinNoiseTurbulence(0.02f, 0.02f, size/100, 0))
            using (var shader = SKShader.CreateCompose(sweep, turbulence))
            {
                var response = await GetWithFilterAsync(size, topic, shader:shader);
                return response;
            }
        }

        #endregion // GetShaderAsync

        #region GetRainbowAsync

        // GET api/imagemanip/rainbow/{size}/{topic}
        [Route("rainbow/{size}/{topic}")]
        [HttpGet]
        public async ValueTask<IActionResult> GetRainbowAsync(int size, string topic)
        {
            var colors = new SKColor[] { SKColors.Red, SKColors.Yellow, SKColors.Green, SKColors.Blue, SKColors.Purple };
            using (var shader = SKShader.CreateLinearGradient(new SKPoint(0, 0), new SKPoint(size, size / 3), colors, null, SKShaderTileMode.Clamp))
            {
                var response = await GetWithFilterAsync(size, topic, shader: shader);
                return response;
            }
        }

        #endregion // GetRainbowAsync

        #region GetWithFilterAsync

        public async ValueTask<IActionResult> GetWithFilterAsync(
            int size,
            string topic,
            SKImageFilter imageFilter = null,
            SKColorFilter colorFilter = null,
            SKShader shader = null,
            SKBlendMode blendMode = SKBlendMode.Overlay)
        {
            string url = string.Format(URL_PATTERN, size, topic);
            var imageInfo = new SKImageInfo(size, size);
            using (var http = new HttpClient())
            {
                var image = await http.GetByteArrayAsync(url).ConfigureAwait(false);                
                using (var outStream = new MemoryStream())
                using (var bitmap = SKBitmap.Decode(image, imageInfo))
                using (var surface = SKSurface.Create(bitmap.Info))
                using (var paint = new SKPaint())
                {
                    SKCanvas canvas = surface.Canvas;
                    canvas.DrawColor(SKColors.White);
                    if (imageFilter != null)
                        paint.ImageFilter = imageFilter;
                    if (colorFilter != null)
                        paint.ColorFilter = colorFilter;

                    // draw the bitmap through the filter
                    var rect = SKRect.Create(imageInfo.Size);
                    canvas.DrawBitmap(bitmap, rect, paint);
                    if (shader != null)
                    {
                        paint.Shader = shader;
                        paint.BlendMode = blendMode;
                        canvas.DrawPaint(paint);
                    }
                    SKData data = surface.Snapshot().Encode(SKEncodedImageFormat.Jpeg, 80);
                    data.SaveTo(outStream);
                    byte[] manipedImage = outStream.ToArray();
                    var response = File(manipedImage, MEDIA_TYPE);
                    return response;
                }
            }
        }

        #endregion // GetWithFilterAsync
    }
}
