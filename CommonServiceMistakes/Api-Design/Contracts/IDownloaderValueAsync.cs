﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bnaya.Samples
{
    public interface IDownloaderValueAsync
    {
        ValueTask<byte[]> DownloadAsync(string url);
    }
}
