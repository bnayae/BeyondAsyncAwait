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
            _uiUpdater = new Progress<byte[]>(image => RawImages.Add(image));
            _uiEffectUpdater = new Progress<byte[]>(image => EffectImages.Add(image));

            _downloader = new TransformBlock<string, byte[]>(DownloadAsync, 
                new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = DOWNLOAD_PARALLELISM });
            var broadcast = new BroadcastBlock<byte[]>(i => i);
            _downloader.LinkTo(broadcast);
            var originPicture = new ActionBlock<byte[]>(m => _uiUpdater.Report(m));
            broadcast.LinkTo(originPicture);

            var merger = new TransformBlock<byte[][], byte[]>(imgs => MergeImages(imgs, "Sequence"));
            var mergerMod = new TransformBlock<byte[][], byte[]>(imgs => MergeImages(imgs, "Mod"));
            var reporter = new ActionBlock<byte[]>(m => _uiEffectUpdater.Report(m));
            var buffer = new BufferBlock<byte[]>();
            broadcast.LinkTo(buffer);
            var options = new ExecutionDataflowBlockOptions { BoundedCapacity = 2 };

            var allEffects = (EffectType[])Enum.GetValues(typeof(EffectType));
            var nonGreedy = new GroupingDataflowBlockOptions { Greedy = false };
            var batches = Enumerable.Range(0, allEffects.Length / 3)
                                .Select(_ => new BatchBlock<byte[]>(3, nonGreedy))
                                .ToArray();
            var batcheMods = Enumerable.Range(0, allEffects.Length / 3)
                                .Select(_ => new BatchBlock<byte[]>(3, nonGreedy))
                                .ToArray();
            int g = 0;
            foreach (EffectType effect in allEffects)
            {
                var effectTransform = new TransformBlock<byte[], byte[]>(
                    img => DoEffectAndUpdate(img, effect), options);
                buffer.LinkTo(effectTransform);
                var batch = batches[g / 3];
                var batchMod = batcheMods[g % 3];
                g++;
                effectTransform.LinkTo(batch);
                effectTransform.LinkTo(batchMod);
                batch.LinkTo(merger);
                batchMod.LinkTo(mergerMod);
                merger.LinkTo(reporter);
                mergerMod.LinkTo(reporter);
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
                        x.Pixelate(12);
                        break;
                    case EffectType.OilPaint:
                        x.OilPaint(5, 15);
                        break;
                    case EffectType.Quantize:
                        x.Quantize();
                        break;
                    case EffectType.Rotate:
                        x.Rotate(RotateMode.Rotate180);
                        break;
                    case EffectType.ColorBlindness:
                        x.ColorBlindness(ColorBlindnessMode.Deuteranomaly);
                        break;
                    case EffectType.DetectEdges:
                        x.DetectEdges();
                        break;
                    case EffectType.Glow:
                        x.Glow();
                        break;
                }

                sw.Stop();
                x.DrawText($"# {effect}",
                    new Font(SystemFonts.Collection.Families.First(), 62),
                    Color.Black, new PointF(33, 33));
                x.DrawText($"# {effect}",
                    new Font(SystemFonts.Collection.Families.First(), 60),
                    Color.White, new PointF(30, 30));
            });
            using (var ms = new MemoryStream())
            {
                imageProcessor.SaveAsJpeg(ms);
                Trace.WriteLine($"# {effect}: {sw.ElapsedMilliseconds:N0}");
                return ms.ToArray();
            }
        }

        private byte[] MergeImages(byte[][] images, string title)
        {
            Image<Rgba32>[] results = images.Select( image =>  Image.Load(image))
                                                    .ToArray();

            using (var imageProcessor = new Image<Rgba32>(IMAGE_SIZE * 3, IMAGE_SIZE))
            using (var outStream = new MemoryStream())
            {
                imageProcessor.Mutate(x =>
                {
                    for (int i = 0; i < results.Length; i++)
                    {
                        Image<Rgba32> img = results[i];
                        x.DrawImage(img, new Point(IMAGE_SIZE * i, 0), 1);
                    }
                    x.DrawText(title,
                        new Font(SystemFonts.Collection.Families.First(), 124),
                        Color.Black, new PointF(IMAGE_SIZE, IMAGE_SIZE / 2 + 30));
                    x.DrawText(title,
                        new Font(SystemFonts.Collection.Families.First(), 120),
                        Color.Orange, new PointF(IMAGE_SIZE + 3, (IMAGE_SIZE / 2 + 30) + 3));
                });
                imageProcessor.SaveAsJpeg(outStream);
                byte[] manipedImage = outStream.ToArray();
                return manipedImage;
            }
        }
    }
}
