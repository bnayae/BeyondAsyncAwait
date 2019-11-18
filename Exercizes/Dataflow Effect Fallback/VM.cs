using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using System.IO;
using SixLabors.Fonts;

namespace ActionBlockUI
{
    class VM
    {
        public ObservableCollection<byte[]> RawImages { get; set; } = new ObservableCollection<byte[]>();
        public ObservableCollection<byte[]> EffectImages { get; set; } = new ObservableCollection<byte[]>();
        private readonly HttpClient _http = new HttpClient();
        private readonly TransformBlock<string, byte[]> _downloader;
        private readonly IProgress<byte[]> _uiUpdater;
        private readonly IProgress<byte[]> _uiEffectUpdater;

        private const int DOWNLOAD_PARALLELISM = 30;
        private const int IMAGE_SIZE = 500;
        private string URL = "https://picsum.photos/" + IMAGE_SIZE + "/";
        public VM()
        {
            _uiUpdater = new Progress<byte[]>(image => RawImages.Insert(0,image));
            _uiEffectUpdater = new Progress<byte[]>(image => EffectImages.Insert(0,image));

            _downloader = new TransformBlock<string, byte[]>(DownloadAsync, 
                new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = DOWNLOAD_PARALLELISM });
            var broadcast = new BroadcastBlock<byte[]>(i => i);
            _downloader.LinkTo(broadcast);
            var originPicture = new ActionBlock<byte[]>(m => _uiUpdater.Report(m));
            broadcast.LinkTo(originPicture);

            var reporter = new ActionBlock<byte[]>(m => _uiEffectUpdater.Report(m));
            var buffer = new BufferBlock<byte[]>();
            broadcast.LinkTo(buffer);
            var options = new ExecutionDataflowBlockOptions { BoundedCapacity = 2 };
            foreach (EffectType effect in (EffectType[])Enum.GetValues(typeof(EffectType)))
            {
                var effectTransform = new TransformBlock<byte[], byte[]>(
                    img => DoEffectAndUpdate(img, effect), options);
                effectTransform.LinkTo(reporter);
                buffer.LinkTo(effectTransform);
            }

            for (int i = 0; i < 500; i++)
            {
                _downloader.Post(URL);
            }
        }

        private async Task<byte[]> DownloadAsync(string url)
        {
            byte[] image = await _http.GetByteArrayAsync(url).ConfigureAwait(false);
            return image;
        }

        private byte[] DoEffectAndUpdate(byte[] image, EffectType effect)
        {
            byte[] buffer = DoEffect(image, effect);
            return buffer;
        }

        private byte[] DoEffect(byte[] image, EffectType effect)
        {
            var sw = Stopwatch.StartNew();
            Image<Rgba32> imageProcessor = Image.Load(image);
            imageProcessor.Mutate(x =>
            {
                switch (effect)
                {
                    case EffectType.Grayscale:
                        x.Grayscale();
                        break;
                    case EffectType.Blur:
                        x.GaussianBlur();
                        break;
                    case EffectType.Pixelate:
                        x.Pixelate();
                        break;
                    case EffectType.OilPaint:
                        x.OilPaint();
                        break;
                    case EffectType.Quantize:
                        x.Quantize();
                        break;
                    case EffectType.Rotate:
                        x.Rotate(RotateMode.Rotate180);
                        break;
                    case EffectType.Kodachrome:
                        x.Kodachrome();
                        break;
                }

                sw.Stop();
                x.DrawText($"# {effect}: {sw.ElapsedMilliseconds:N0}", 
                    new Font( SystemFonts.Collection.Families.First(), 40), 
                    Color.White, new PointF(30, 30));
            });
            using (var ms = new MemoryStream())
            {
                imageProcessor.SaveAsJpeg(ms);
                Trace.WriteLine($"# {effect}: {sw.ElapsedMilliseconds:N0}");
                return ms.ToArray();
            }
        }
    }
}
