using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Bnaya.Samples
{
    public class Downloader: IDownloader
    {
        public byte[] Download(string url)
        {
            using (var http = new HttpClient())
            {
                // must block (cause thread-pool starvation)
                var result = http.GetByteArrayAsync(url).Result; 
                return result;
            }
        }
    }
}
