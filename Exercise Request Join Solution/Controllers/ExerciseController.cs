using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Exercise_Request_Join_Solution.Controllers
{
    [Route("api/[controller]")]
    public class ExerciseController : Controller
    {
        private static readonly ConcurrentDictionary<string, Correlation> _storage = new ConcurrentDictionary<string, Correlation>();

        // GET api/exercise/a/{correlationKey}
        [HttpGet]
        [Route("a/{correlationKey}")]
        public Task<string> GetA(string correlationKey)
        {
            Correlation state = _storage.GetOrAdd(correlationKey, c => new Correlation(c));
            return state.Increment(AB.A);
        }
        // GET api/exercise/a/{correlationKey}
        [HttpGet]
        [Route("b/{correlationKey}")]
        public Task<string> GetB(string correlationKey)
        {
            Correlation state = _storage.GetOrAdd(correlationKey, c => new Correlation(c));
            return state.Increment(AB.B);
        }

        private class Correlation
        {
            private readonly object _gate = new object();
            private int _a = 0;
            private int _b = 0;
            private readonly string _correlationKey;

            private TaskCompletionSource<string> _taskSemantic { get; } = new TaskCompletionSource<string>();

            public Correlation(string correlationKey)
            {
                _correlationKey = correlationKey;
            }

            #region With Lock

            public Task<string> Increment(AB kind)
            {
                lock (_gate)
                {
                    switch (kind)
                    {
                        case AB.A:
                            if (_b == 0)
                                _a++;
                            else
                                _taskSemantic.TrySetResult($"{_correlationKey} B, A");
                            break;
                        case AB.B:
                            if (_a == 0)
                                _b++;
                            else
                                _taskSemantic.TrySetResult($"{_correlationKey} B, A");
                            break;
                    }
                    if (_taskSemantic.Task.IsCompleted)
                    {
                        _b = _a = 0;
                        if (_storage.TryRemove(_correlationKey, out Correlation removed))
                        {
                            if (removed != this)
                                Trace.WriteLine("Race should be handle");
                        }
                    }
                }
                return _taskSemantic.Task;
            }

            #endregion // With Lock
        }

        private enum AB
        {
            A,
            B
        }
    }
}
