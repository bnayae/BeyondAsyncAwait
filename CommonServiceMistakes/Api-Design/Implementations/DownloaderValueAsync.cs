﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Bnaya.Samples
{
    public class DownloaderValueAsync: IDownloaderValueAsync
    {
        public async ValueTask<byte[]> DownloadAsync(string url)
        {
            using (var http = new HttpClient())
            {
                // must block (cause thread-pool starvation)
                var result = await http.GetByteArrayAsync(url); 
                return result;
            }
        }
    }
}
