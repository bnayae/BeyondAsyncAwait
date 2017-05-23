#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

#endregion // Using

namespace Bnaya.Samples
{
    public static class FileSystemWatcherTask
    {
        public static Task<string> ToTask(
            this FileSystemWatcher instance, Action action)
        {
            var tcs = new TaskCompletionSource<string>();
            instance.EnableRaisingEvents = true;
            instance.Filter = "*.txt";
            instance.Deleted += (s, e) =>
                {   
                    action();
                    tcs.SetResult(e.FullPath);
                };

            return tcs.Task;
        }
    }
}
