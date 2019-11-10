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
        // https://loremipsum.io/21-of-the-best-placeholder-image-generators/
        //private const string URL = "https://loremflickr.com/640/360";
        //private const string URL = "https://source.unsplash.com/1200x1200/?woman";
        //private const string URL = "https://picsum.photos/1200";
        private const string URL = "https://cdn1.medicalnewstoday.com/content/images/articles/322/322868/golden-retriever-puppy.jpg";

        static void Main(string[] args)
        {
            //Task t = JoinSimpleDownloadAndSaveAsync();
            Task t = DownloadForkEffectJoinSave(0);
            //Task t = DownloadForkEffectJoinSaveAll();

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
            //var tasks = Enumerable.Range(0, 20)
            //                .Select(i => SimpleDownloadAndSaveAsync(i));
            //await Task.WhenAll(tasks);
            Console.WriteLine("Both complete");
        }

        #endregion // JoinSimpleDownloadAndSaveAsync

        #region SimpleDownloadAndSaveAsync

        private static async Task SimpleDownloadAndSaveAsync(int index)
        {
            byte[] image =  await DownloadAsync(URL);
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
            using (var fs = new FileStream(name, FileMode.Create, 
                                        FileAccess.Write, FileShare.None, 
                                        4096, FileOptions.Asynchronous))
            {
                Console.Write(" Saving ");
                await fs.WriteAsync(data, 0, data.Length);
                Console.Write(" Saved ");
            }
        }

        #endregion // SaveAsync

        #region DownloadForkEffectJoinSave

        private static async Task DownloadForkEffectJoinSaveAll()
        {
            var tasks = Enumerable.Range(0, 20)
                .Select(DownloadForkEffectJoinSave);
            await Task.WhenAll(tasks);
        }
        private static async Task DownloadForkEffectJoinSave(int i)
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
            await SaveAsync($"Merged Image {i}.jpg", mergedImage);
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
