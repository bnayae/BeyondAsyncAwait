using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Bnaya.Samples
{
    public class DownloaderAsync: IDownloaderAsync
    {
        public async Task<byte[]> DownloadAsync(string url)
        {
            using (var http = new HttpClient())
            {
                var result = await http.GetByteArrayAsync(url).ConfigureAwait(false);
                return result;
            }
        }
    }
}
