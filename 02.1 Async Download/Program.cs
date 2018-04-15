using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Drawing;
using SixLabors.ImageSharp.Processing.Effects;
using SixLabors.ImageSharp.Processing.Filters;
using SixLabors.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Math;

namespace _02._1_Async_Download
{
    class Program
    {
        private const string URL = "https://source.unsplash.com/1200x1200/?woman";

        static void Main(string[] args)
        {
            Task t = JoinSimpleDownloadAndSaveAsync();
            //Task t = DownloadForkEffectJoinSave();

            while (!t.IsCompleted)
            {
                Console.Write(".");
                Thread.Sleep(10);
            }
            Console.WriteLine("Done");
            Console.ReadKey();
        }

        #region JoinSimpleDownloadAndSaveAsync

        private static async Task JoinSimpleDownloadAndSaveAsync()
        {
            Task t1 = SimpleDownloadAndSaveAsync(1);
            Task t2 = SimpleDownloadAndSaveAsync(2);
            await Task.WhenAll(t1, t2);
            Console.WriteLine("Both complete");
        }

        #endregion // JoinSimpleDownloadAndSaveAsync

        #region SimpleDownloadAndSaveAsync

        private static async Task SimpleDownloadAndSaveAsync(int index)
        {
            byte[] image = await DownloadAsync(URL);
            await SaveAsync($"Image {index}.jpg", image);
        }

        #endregion // SimpleDownloadAndSaveAsync

        #region DownloadAsync

        private static async Task<byte[]> DownloadAsync(string url)
        {
            using (var http = new HttpClient())
            {
                Console.Write(" Downloading ");
                byte[] data = await http.GetByteArrayAsync(url);
                Console.Write(" Downloaded ");
                return data;
            }
        }

        #endregion // DownloadAsync

        #region SaveAsync

        private static async Task SaveAsync(string name, byte[] data)
        {
            using (var fs = new FileStream(name, FileMode.Create, FileAccess.Write, FileShare.None, 4096, FileOptions.Asynchronous))
            {
                Console.Write(" Saving ");
                await fs.WriteAsync(data, 0, data.Length);
                Console.Write(" Saved ");
            }
        }

        #endregion // SaveAsync

        #region DownloadForkEffectJoinSave

        private static async Task DownloadForkEffectJoinSave()
        {
            // download
            byte[] image = await DownloadAsync(URL);

            // manipulate the image
            Image<Rgba32>[] images = await Task.WhenAll(
                    Task.Run(() => Pixelate(image)),
                    Task.Run(() => Grayscale(image))
                );

            // merge the images
            byte[] mergedImage = MergeImages(images[0], images[1]);
            await SaveAsync("Merged Image.jpg", mergedImage);
        }

        #endregion // DownloadForkEffectJoinSave

        #region Pixelate

        public static Image<Rgba32> Pixelate(byte[] image)
        {
            Console.Write(" Pixelate start ");
            Image<Rgba32> imageProcessor = Image.Load(image);
            imageProcessor.Mutate(x => x.Pixelate(10));
            Console.Write(" Pixelate complete ");
            return imageProcessor;

        }

        #endregion // Pixelate

        #region Grayscale

        public static Image<Rgba32> Grayscale(byte[] image)
        {
            Console.Write(" Grayscale start ");
            Image<Rgba32> imageProcessor = Image.Load(image);
            imageProcessor.Mutate(x => x.Grayscale());
            Console.Write(" Grayscale complete ");
            return imageProcessor;

        }

        #endregion // Grayscale

        #region MergeImages

        public static byte[] MergeImages(
            Image<Rgba32> img0,
            Image<Rgba32> img1)
        {
            Console.Write(" Merging ");
            using (var imageProcessor = new Image<Rgba32>(
                                            img0.Width + img1.Width,
                                            Max(img0.Height, img1.Height)))
            using (var outStream = new MemoryStream())
            {
                imageProcessor.Mutate(x => x
                                    .DrawImage(img0, opacity: 1)
                                    .DrawImage(img1, opacity: 1, location: new Point(img0.Width, 0)));
                imageProcessor.SaveAsJpeg(outStream);
                Console.Write(" Merged ");
                return outStream.ToArray();
            }
        }

        #endregion // MergeImages
    }
}
