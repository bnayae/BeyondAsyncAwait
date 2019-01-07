using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Effects;
using SixLabors.ImageSharp.Processing.Filters;
using SixLabors.Primitives;
using SixLabors.ImageSharp.Processing.Drawing;
using static System.Math;

namespace ImageProcessingFlowFxSolution
{
    static class Program
    {
        private const string URL = "https://source.unsplash.com/1200x1200/?dog/";
        private static TransformBlock<int, byte[]> _downloader;
        private static BroadcastBlock<byte[]> _broadcast;
        private static TransformBlock<byte[], Image<Rgba32>> _effectA;
        private static TransformBlock<byte[], Image<Rgba32>> _effectB;
        private static BatchBlock<Image<Rgba32>> _batch;
        private static TransformBlock<Image<Rgba32>[], byte[]> _merge;
        private static ActionBlock<byte[]> _save;

        static async Task Main()
        {
            if (!Directory.Exists("Images"))
                Directory.CreateDirectory("Images");

            //var opt = new ExecutionDataflowBlockOptions { BoundedCapacity = 1 };
            _downloader = new TransformBlock<int, byte[]>(i => DownloadAsync(i), new ExecutionDataflowBlockOptions { BoundedCapacity = 2 });

            _broadcast = new BroadcastBlock<byte[]>(m => m);

            _effectA = new TransformBlock<byte[], Image<Rgba32>>(
                            m => ApplyEffect(m, "Invert+Kodachrome", x => x.Invert().Kodachrome()));
            _effectB = new TransformBlock<byte[], Image<Rgba32>>(
                            m => ApplyEffect(m, "Grayscale", x => x.Grayscale()));
            _batch = new BatchBlock<Image<Rgba32>>(2, new GroupingDataflowBlockOptions { Greedy = false });
            _merge = new TransformBlock<Image<Rgba32>[], byte[]>(m => MergeAsync(m));
            _save = new ActionBlock<byte[]>(SaveAsync);

            _downloader.LinkTo(_broadcast);

            _broadcast.LinkTo(_effectA);
            _broadcast.LinkTo(_effectB);

            _effectA.LinkTo(_batch);
            _effectB.LinkTo(_batch);
            _batch.LinkTo(_merge);
            _merge.LinkTo(_save);

            for (int i = 0; i < 15; i++)
            {
                //_downloader.Post(i);
                await _downloader.SendAsync(i);
            }
            Console.ReadKey();
        }

        #region DownloadAsync

        public static async Task<byte[]> DownloadAsync(int imageIndex)
        {
            Console.WriteLine($"Downloading {imageIndex}");
            using (var http = new HttpClient())
            {
                var image = await http.GetByteArrayAsync(URL);
                Console.WriteLine($"Downloaded {imageIndex}");
                return image;
            }
        }

        #endregion // DownloadAsync

        #region ApplyEffect

        private static Image<Rgba32> ApplyEffect(
            byte[] image,
            string title,
            Action<IImageProcessingContext<Rgba32>> operation)

        {
            Console.WriteLine($"Effect: {title} is starting");

            Image<Rgba32> imageProcessor = Image.Load(image);
            imageProcessor.Mutate(x => operation?.Invoke(x));

            Console.WriteLine($"Effect: {title} is complete");
            return imageProcessor;
        }

        #endregion // ApplyEffect

        #region MergeAsync

        private static Task<byte[]> MergeAsync(Image<Rgba32>[] images)
        {
            Console.WriteLine("Merging");
            using (Image<Rgba32> img0 = images[0])
            using (Image<Rgba32> img1 = images[1])
            using (var imageProcessor = 
                new Image<Rgba32>(img0.Width + img1.Width,
                                Max(img0.Height, img1.Height)))
            using (var outStream = new MemoryStream())
            {
                imageProcessor.Mutate(x => x
                                    .DrawImage(img0, 1)
                                    .DrawImage(img1, 1, new Point(img0.Width, 0)));
                imageProcessor.SaveAsJpeg(outStream);
                byte[] merged = outStream.ToArray();
                Console.WriteLine("Merged");
                return Task.FromResult(merged);
            }
        }

        #endregion // MergeAsync

        #region SaveAsync

        private static async Task SaveAsync(byte[] image)
        {
            Console.WriteLine("Saving");
            using (var fs = new FileStream($@"Images\Image_{Guid.NewGuid()}.jpg",
                                        FileMode.Create, FileAccess.Write,
                                        FileShare.None, 4096, FileOptions.Asynchronous))
            {
                await fs.WriteAsync(image, 0, image.Length);
            }
            Console.WriteLine("Saved");
        }

        #endregion // SaveAsync
    }
}
