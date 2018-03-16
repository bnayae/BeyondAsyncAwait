using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bnaya.Samples
{
    public static class Extensions
    {
        public static FileSystemWatcherAwaiter GetAwaiter(
            this FileSystemWatcher fsw)
        {
            return new FileSystemWatcherAwaiter(fsw);
        }
    }
}
