using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Exercise_Request_Join_Solution.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private static readonly ConcurrentDictionary<string, (string AB, TaskCompletionSource<string> TaskSemantic, int Count)> _map =
            new ConcurrentDictionary<string, (string AB, TaskCompletionSource<string> TaskSemantic, int Count)>();
        
        // GET api/values
        [HttpGet]
        [Route("a/{correlateKey}")]
        public Task<string> GetAAsync(string correlateKey)
        {
            var tuple = _map.AddOrUpdate(correlateKey,
                            c => ("A", new TaskCompletionSource<string>(), 1),
                            (k, v) =>
                            {
                                if (v.AB == "B")
                                {
                                    v.TaskSemantic.TrySetResult($"{correlateKey} B, A, Count = {v.Count}");
                                    return v;
                                }
                                return (v.AB, v.TaskSemantic, v.Count + 1);
                            });
            //var tuple = _map.GetOrAdd(correlateKey, c => ("A", new TaskCompletionSource<string>(), 1));
            //if (tuple.AB == "B")
            //    tuple.TaskSemantic.TrySetResult($"{correlateKey} B, A, Count = {tuple.Count}");
            //else
            //    Interlocked.Increment(ref tuple.Count);
            return tuple.TaskSemantic.Task;
        }

        [HttpGet]
        [Route("B/{correlateKey}")]
        public Task<string> GetBAsync(string correlateKey)
        {
            var tuple = _map.GetOrAdd(correlateKey, c => ("B", new TaskCompletionSource<string>(), 1));
            if (tuple.AB == "A")
                tuple.TaskSemantic.TrySetResult($"{correlateKey} A, B, Count = {tuple.Count}");
            else
                Interlocked.Increment(ref tuple.Count); return tuple.TaskSemantic.Task;
        }
    }
}
