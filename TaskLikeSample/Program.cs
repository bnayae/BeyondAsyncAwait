using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bnaya.Samples
{
    class Program
    {
        private static ObjectCache cache = MemoryCache.Default;

        static async Task Main(string[] args)
        {
            Write("Start");
            await ExecTaskLike().ConfigureAwait(false);
            //await ExecDataTask().ConfigureAwait(false);
            Console.ReadKey();
        }

        //private static async Task ExecDataTask()
        //{
        //    string s = await GetAsync(5).ConfigureAwait(false);
        //    string s1 = await GetAsync(5).ConfigureAwait(false);
        //}

        //private CommonData GetData() => new CommonData(1, "sync");
        //private async DataTask<CommonData> GetDataAsync()
        //{
        //    await Task.Delay(1).ConfigureAwait(false);
        //    return new CommonData(2, "async");
        //}

        private static async Task ExecTaskLike()
        {
            string s = await GetAsync(5).ConfigureAwait(false);
            string s1 = await GetAsync(5).ConfigureAwait(false);
        }

        private static async TaskLike<string> GetAsync(int times)
        {
            string key = times.ToString();
            object cached = cache.Get(key);
            if (cached != null)
            {
                Write("Cached");
                return (string)cached;
            }
            await Task.Delay(5000).ConfigureAwait(false);
            string value = new string('*', times);
            var policy = new CacheItemPolicy
            {
                SlidingExpiration = TimeSpan.FromSeconds(10)
            };
            cache.Set(key, value, policy);
            Write("Not Cached");
            return value;
        }


        private static void Write(object title)
        {
            var trd = Thread.CurrentThread;
            Console.WriteLine($"{title} [{trd.ManagedThreadId}]");
        }

        
    }
}
