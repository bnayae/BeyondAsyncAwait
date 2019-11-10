using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WindowsDownloadAsync
{
    public class Downloader
    {
        // https://loremipsum.io/21-of-the-best-placeholder-image-generators/
        //private const string URL = "https://loremflickr.com/640/360";
        //private const string URL = "http://lorempixel.com/800/800/animals/";
        //private const string URL = "https://picsum.photos/800";
        private const string URL = "https://source.unsplash.com/400x400";

        public static async Task<Image> DownloadAsync()
        {
            using (var client = new HttpClient())
            {
                Stream img = await client.GetStreamAsync(URL)
                                        .ConfigureAwait(false);
                var picture = Image.FromStream(img);
                return picture;
            }
        }
    }
}
